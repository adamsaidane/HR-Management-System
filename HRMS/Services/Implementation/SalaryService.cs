using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class SalaryService : ISalaryService
{
    private readonly IUnitOfWork _unitOfWork;

    public SalaryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> GetCurrentSalaryAsync(int employeeId)
    {
        var currentSalary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employeeId);
        return currentSalary?.BaseSalary ?? 0;
    }

    public async Task<IEnumerable<Salary>> GetSalaryHistoryAsync(int employeeId)
    {
        return await _unitOfWork.Salaries.GetSalaryHistoryAsync(employeeId);
    }

    public async Task AddSalaryAsync(Salary salary)
    {
        salary.CreatedDate = DateTime.Now;
        await _unitOfWork.Salaries.AddAsync(salary);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateSalaryAsync(int employeeId, decimal newSalary, string justification)
    {
        // Clôturer l'ancien salaire
        var currentSalary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employeeId);

        if (currentSalary != null)
        {
            currentSalary.EndDate = DateTime.Now.AddDays(-1);
            _unitOfWork.Salaries.Update(currentSalary);
        }

        // Créer le nouveau salaire
        var newSalaryRecord = new Salary
        {
            EmployeeId = employeeId,
            BaseSalary = newSalary,
            EffectiveDate = DateTime.Now,
            Justification = justification,
            CreatedDate = DateTime.Now
        };

        await _unitOfWork.Salaries.AddAsync(newSalaryRecord);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Bonus>> GetBonusesByEmployeeAsync(int employeeId)
    {
        return await _unitOfWork.Bonuses.GetByEmployeeAsync(employeeId);
    }

    public async Task AddBonusAsync(Bonus bonus)
    {
        bonus.CreatedDate = DateTime.Now;
        await _unitOfWork.Bonuses.AddAsync(bonus);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalBonusesAsync(int employeeId, int year)
    {
        return await _unitOfWork.Bonuses.GetTotalBonusesAsync(employeeId, year);
    }

    public async Task<IEnumerable<Benefit>> GetAllBenefitsAsync()
    {
        return await _unitOfWork.Benefits.GetAllAsync();
    }

    public async Task<IEnumerable<EmployeeBenefit>> GetEmployeeBenefitsAsync(int employeeId)
    {
        return await _unitOfWork.EmployeeBenefits.GetEmployeeBenefitsAsync(employeeId);
    }

    public async Task AssignBenefitToEmployeeAsync(int employeeId, int benefitId, DateTime startDate)
    {
        var employeeBenefit = new EmployeeBenefit
        {
            EmployeeId = employeeId,
            BenefitId = benefitId,
            StartDate = startDate
        };

        await _unitOfWork.EmployeeBenefits.AddAsync(employeeBenefit);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveBenefitFromEmployeeAsync(int employeeBenefitId, DateTime endDate)
    {
        var employeeBenefit = await _unitOfWork.EmployeeBenefits.GetByIdAsync(employeeBenefitId);
        if (employeeBenefit != null)
        {
            employeeBenefit.EndDate = endDate;
            _unitOfWork.EmployeeBenefits.Update(employeeBenefit);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalBenefitsValueAsync(int employeeId)
    {
        return await _unitOfWork.EmployeeBenefits.GetTotalBenefitsValueAsync(employeeId);
    }

    public async Task<decimal> CalculateGrossSalaryAsync(int employeeId)
    {
        var baseSalary = await GetCurrentSalaryAsync(employeeId);
        var benefits = await GetTotalBenefitsValueAsync(employeeId);
        return baseSalary + benefits;
    }

    public async Task<decimal> CalculateTotalCompensationAsync(int employeeId, int year)
    {
        var baseSalary = await GetCurrentSalaryAsync(employeeId) * 12;
        var bonuses = await GetTotalBonusesAsync(employeeId, year);
        var benefits = await GetTotalBenefitsValueAsync(employeeId) * 12;
        return baseSalary + bonuses + benefits;
    }
}