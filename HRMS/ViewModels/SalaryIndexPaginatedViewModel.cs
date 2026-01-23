using HRMS.Models;

namespace HRMS.ViewModels;

public class SalaryIndexPaginatedViewModel
{
    public PaginatedList<Employee> Employees { get; set; }
    public List<Department> Departments { get; set; } = new();
    public string? SearchString { get; set; }
    public int? DepartmentId { get; set; }
    public decimal TotalMasseSalariale { get; set; }
    public decimal AverageSalary { get; set; }
    public int TotalEmployees { get; set; }
}