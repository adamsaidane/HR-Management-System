using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync()
    {
        return await _unitOfWork.Equipments.GetAllAsync();
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
        await _unitOfWork.Equipments.AddAsync(equipment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateEquipmentAsync(Equipment equipment)
    {
        equipment.ModifiedDate = DateTime.Now;
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
}