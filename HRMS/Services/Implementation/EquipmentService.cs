using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeService _employeeService;

    public EquipmentService(IUnitOfWork unitOfWork, IEmployeeService employeeService)
    {
        _unitOfWork = unitOfWork;
        _employeeService = employeeService;
    }

    public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync()
    {
        return await _unitOfWork.Equipments.GetAllWithAssignmentsAsync();
    }

    public async Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync()
    {
        return await _unitOfWork.Equipments.GetAvailableEquipmentAsync();
    }

    public async Task<Equipment?> GetEquipmentByIdAsync(int id)
    {
        return await _unitOfWork.Equipments.GetByIdWithHistoryAsync(id);
    }

    public async Task CreateEquipmentAsync(Equipment equipment)
    {
        equipment.CreatedDate = DateTime.Now;
        equipment.ModifiedDate = DateTime.Now;
        equipment.Status = EquipmentStatus.Disponible;
        await _unitOfWork.Equipments.AddAsync(equipment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateEquipmentAsync(Equipment equipment)
    {
        var existingEquipment = await _unitOfWork.Equipments.GetByIdWithHistoryAsync(equipment.EquipmentId);
        if (existingEquipment == null)
            throw new Exception("Équipement non trouvé");

        var oldStatus = existingEquipment.Status;

        // Update properties from the incoming equipment object
        existingEquipment.EquipmentType = equipment.EquipmentType;
        existingEquipment.Brand = equipment.Brand;
        existingEquipment.Model = equipment.Model;
        existingEquipment.SerialNumber = equipment.SerialNumber;
        existingEquipment.PurchaseDate = equipment.PurchaseDate;
        existingEquipment.Status = equipment.Status;
        existingEquipment.ModifiedDate = DateTime.Now;

        // Handle status changes if status was modified
        if (oldStatus != equipment.Status)
        {
            await HandleEquipmentStatusChangeAsync(existingEquipment, oldStatus, equipment.Status);
        }
        else
        {
            // If status didn't change, still save the equipment
            _unitOfWork.Equipments.Update(existingEquipment);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Handles status changes:
    /// - If changing from Affecté to Disponible, mark the assignment as returned
    /// </summary>
    private async Task HandleEquipmentStatusChangeAsync(Equipment equipment, EquipmentStatus oldStatus, EquipmentStatus newStatus)
    {
        if (oldStatus == EquipmentStatus.Affecté && newStatus == EquipmentStatus.Disponible)
        {
            // Mark current active assignment as returned
            if (equipment.EquipmentAssignments != null)
            {
                var activeAssignment = equipment.EquipmentAssignments.FirstOrDefault(ea => ea.ReturnDate == null);
                
                if (activeAssignment != null)
                {
                    activeAssignment.ReturnDate = DateTime.Now;
                    _unitOfWork.EquipmentAssignments.Update(activeAssignment);
                }
            }
        }
        
        // Update equipment in all cases
        _unitOfWork.Equipments.Update(equipment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteEquipmentAsync(int id)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(id);
        if (equipment != null)
        {
            _unitOfWork.Equipments.Remove(equipment);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<EquipmentAssignment>> GetEmployeeEquipmentAsync(int employeeId)
    {
        return await _unitOfWork.EquipmentAssignments.GetEmployeeEquipmentAsync(employeeId);
    }

    public async Task<IEnumerable<EquipmentAssignment>> GetEquipmentHistoryAsync(int equipmentId)
    {
        return await _unitOfWork.EquipmentAssignments.GetEquipmentHistoryAsync(equipmentId);
    }

    public async Task AssignEquipmentAsync(int equipmentId, int employeeId, string condition, string notes)
    {
        var assignment = new EquipmentAssignment
        {
            EquipmentId = equipmentId,
            EmployeeId = employeeId,
            AssignmentDate = DateTime.Now,
            Condition = condition,
            Notes = notes
        };

        await _unitOfWork.EquipmentAssignments.AddAsync(assignment);

        // Mettre à jour le statut de l'équipement
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(equipmentId);
        if (equipment != null)
        {
            equipment.Status = EquipmentStatus.Affecté;
            equipment.ModifiedDate = DateTime.Now;
            _unitOfWork.Equipments.Update(equipment);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReturnEquipmentAsync(int assignmentId, DateTime returnDate, string condition)
    {
        var assignment = await _unitOfWork.EquipmentAssignments
            .FirstOrDefaultAsync(ea => ea.AssignmentId == assignmentId);

        if (assignment != null)
        {
            assignment.ReturnDate = returnDate;
            assignment.Condition = condition;
            _unitOfWork.EquipmentAssignments.Update(assignment);

            // Mettre à jour le statut de l'équipement
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(assignment.EquipmentId);
            if (equipment != null)
            {
                equipment.Status = EquipmentStatus.Disponible;
                equipment.ModifiedDate = DateTime.Now;
                _unitOfWork.Equipments.Update(equipment);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalEquipmentAsync()
    {
        return await _unitOfWork.Equipments.CountAsync();
    }

    public async Task<int> GetAssignedEquipmentCountAsync()
    {
        return await _unitOfWork.Equipments.CountAsync(e => e.Status == EquipmentStatus.Affecté);
    }

    public async Task<int> GetAvailableEquipmentCountAsync()
    {
        return await _unitOfWork.Equipments.CountAsync(e => e.Status == EquipmentStatus.Disponible);
    }
    
    public async Task<EquipmentIndexViewModel> GetEquipmentIndexViewModelAsync(string equipmentType, EquipmentStatus? status)
    {
        var equipment = await GetAllEquipmentAsync();

        if (!string.IsNullOrEmpty(equipmentType))
            equipment = equipment.Where(e => e.EquipmentType == equipmentType);

        if (status.HasValue)
            equipment = equipment.Where(e => e.Status == status);

        return new EquipmentIndexViewModel
        {
            Equipment = equipment.ToList(),
            EquipmentTypes = equipment.Select(e => e.EquipmentType).Distinct().ToList()
        };
    }

    public async Task<EquipmentPaginatedIndexViewModel> GetEquipmentPaginatedIndexViewModelAsync(string equipmentType, EquipmentStatus? status, int pageIndex = 1,
        int pageSize = 10)
    {
        var equipment = await GetAllEquipmentAsync();
        var allEquipmentTypes = equipment.Select(e => e.EquipmentType).Distinct().ToList();

        if (!string.IsNullOrEmpty(equipmentType))
            equipment = equipment.Where(e => e.EquipmentType == equipmentType);

        if (status.HasValue)
            equipment = equipment.Where(e => e.Status == status);

        // Convert to list before pagination to ensure all data is loaded
        var equipmentList = equipment.ToList();

        return new EquipmentPaginatedIndexViewModel()
        {
            Equipment = PaginatedList<Equipment>.Create(equipmentList, pageIndex, pageSize),
            EquipmentTypes = allEquipmentTypes
        };
    }

    public async Task<EquipmentAssignmentViewModel> GetEquipmentAssignmentViewModelAsync()
    {
        return new EquipmentAssignmentViewModel
        {
            AvailableEquipment = (await GetAvailableEquipmentAsync()).ToList(),
            Employees = (await _employeeService.GetEmployeesByStatusAsync(EmployeeStatus.Actif)).ToList()
        };
    }

    public async Task<Equipment?> GetEquipmentWithAssignmentAsync(int equipmentId)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(equipmentId);
        return equipment;
    }
}
