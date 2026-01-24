using HRMS.Models;

namespace HRMS.ViewModels;

public class DepartmentDetailsViewModel
{
    public Department Department { get; set; }
    public List<PositionWithEmployees> Positions { get; set; } = new();
    public int TotalEmployees { get; set; }
    public decimal TotalSalaryMass { get; set; }
    public decimal AverageSalary { get; set; }
}

public class PositionWithEmployees
{
    public int PositionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal BaseSalary { get; set; }
    public int EmployeeCount { get; set; }
    public List<EmployeeBasicInfo> Employees { get; set; } = new();
}

public class EmployeeBasicInfo
{
    public int EmployeeId { get; set; }
    public string Matricule { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string PhotoPath { get; set; }
    public DateTime HireDate { get; set; }
    public string Status { get; set; }
}
