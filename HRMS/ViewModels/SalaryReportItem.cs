using HRMS.Models;

namespace HRMS.ViewModels;

public class SalaryReportItem
{
    public Employee Employee { get; set; } = null!;
    public decimal CurrentSalary { get; set; }
    public decimal TotalBenefits { get; set; }
    public decimal GrossSalary { get; set; }
}