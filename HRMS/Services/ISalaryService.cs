using HRMS.Models;

namespace HRMS.Service;

public interface ISalaryService
{
    Task<decimal> GetCurrentSalaryAsync(int employeeId);
    Task<IEnumerable<Salary>> GetSalaryHistoryAsync(int employeeId);
    Task AddSalaryAsync(Salary salary);
    Task UpdateSalaryAsync(int employeeId, decimal newSalary, string justification);
    Task<IEnumerable<Bonus>> GetBonusesByEmployeeAsync(int employeeId);
    Task AddBonusAsync(Bonus bonus);
    Task<decimal> GetTotalBonusesAsync(int employeeId, int year);
    Task<IEnumerable<Benefit>> GetAllBenefitsAsync();
    Task<IEnumerable<EmployeeBenefit>> GetEmployeeBenefitsAsync(int employeeId);
    Task AssignBenefitToEmployeeAsync(int employeeId, int benefitId, DateTime startDate);
    Task RemoveBenefitFromEmployeeAsync(int employeeBenefitId, DateTime endDate);
    Task<decimal> GetTotalBenefitsValueAsync(int employeeId);
    Task<decimal> CalculateGrossSalaryAsync(int employeeId);
    Task<decimal> CalculateTotalCompensationAsync(int employeeId, int year);
}