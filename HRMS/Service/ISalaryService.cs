using HRMS.Models;

namespace HRMS.Service;

public interface ISalaryService
{
    // Salaires
    decimal GetCurrentSalary(int employeeId);
    IEnumerable<Salary> GetSalaryHistory(int employeeId);
    void AddSalary(Salary salary);
    void UpdateSalary(int employeeId, decimal newSalary, string justification);

    // Primes
    IEnumerable<Bonus> GetBonusesByEmployee(int employeeId);
    void AddBonus(Bonus bonus);
    decimal GetTotalBonuses(int employeeId, int year);

    // Avantages
    IEnumerable<Benefit> GetAllBenefits();
    IEnumerable<EmployeeBenefit> GetEmployeeBenefits(int employeeId);
    void AssignBenefitToEmployee(int employeeId, int benefitId, DateTime startDate);
    void RemoveBenefitFromEmployee(int employeeBenefitId, DateTime endDate);
    decimal GetTotalBenefitsValue(int employeeId);

    // Calculs
    decimal CalculateGrossSalary(int employeeId);
    decimal CalculateTotalCompensation(int employeeId, int year);
}