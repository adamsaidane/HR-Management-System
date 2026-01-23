using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISalaryService _salaryService;
    private readonly IEmployeeService _employeeService;

    public PromotionService(IUnitOfWork unitOfWork, ISalaryService salaryService, IEmployeeService employeeService)
    {
        _unitOfWork = unitOfWork;
        _salaryService = salaryService;
        _employeeService = employeeService;
    }

    public async Task<IEnumerable<Promotion>> GetAllPromotionsAsync()
    {
        return await _unitOfWork.Promotions.GetAllWithDetailsAsync();
    }
    
    public async Task<PaginatedList<Promotion>> GetAllPromotionsPaginatedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var promotions = await _unitOfWork.Promotions.GetAllWithDetailsAsync();
        return PaginatedList<Promotion>.Create(promotions, pageIndex, pageSize);
    }

    public async Task<IEnumerable<Promotion>> GetEmployeePromotionsAsync(int employeeId)
    {
        return await _unitOfWork.Promotions.GetEmployeePromotionsAsync(employeeId);
    }

    public async Task CreatePromotionAsync(Promotion promotion)
    {
        promotion.CreatedDate = DateTime.Now;
        await _unitOfWork.Promotions.AddAsync(promotion);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ProcessPromotionAsync(int employeeId, int newPositionId, decimal newSalary, string justification)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
            throw new Exception("Employé introuvable");

        var currentSalary = await _salaryService.GetCurrentSalaryAsync(employeeId);

        // Créer l'enregistrement de promotion
        var promotion = new Promotion
        {
            EmployeeId = employeeId,
            OldPositionId = employee.PositionId,
            NewPositionId = newPositionId,
            OldSalary = currentSalary,
            NewSalary = newSalary,
            PromotionDate = DateTime.Now,
            Justification = justification
        };

        await CreatePromotionAsync(promotion);

        // Mettre à jour l'employé
        employee.PositionId = newPositionId;
        employee.ModifiedDate = DateTime.Now;
        _unitOfWork.Employees.Update(employee);

        // Mettre à jour le salaire
        await _salaryService.UpdateSalaryAsync(employeeId, newSalary, $"Promotion: {justification}");

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<PromotionFormViewModel> GetPromotionFormViewModelAsync()
    {
        return new PromotionFormViewModel
        {
            Employees = (await _employeeService.GetEmployeesByStatusAsync(EmployeeStatus.Actif)).ToList(),
            Positions = await _employeeService.GetAllPositionsAsync(),
            PromotionDate = DateTime.Today
        };
    }
}