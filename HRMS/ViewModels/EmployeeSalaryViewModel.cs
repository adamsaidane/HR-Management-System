using HRMS.Models;

namespace HRMS.ViewModels;

public class EmployeeSalaryViewModel
{
    public Employee Employee { get; set; } = null!;
    public decimal? CurrentSalary { get; set; }
    public List<Salary> SalaryHistory { get; set; } = new();
    public List<Bonus> Bonuses { get; set; } = new();
    public List<EmployeeBenefit> Benefits { get; set; } = new();
    public List<Benefit> AllBenefits { get; set; } = new();
    public decimal? TotalBenefits { get; set; }
    public decimal? GrossSalary { get; set; }
}