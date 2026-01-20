using HRMS.Models;

namespace HRMS.ViewModels;

public class DepartmentSalaryReport
{
    public Department Department { get; set; } = null!;
    public int EmployeeCount { get; set; }
    public decimal TotalSalary { get; set; }
    public decimal AverageSalary { get; set; }
}