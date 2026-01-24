using HRMS.Models;

namespace HRMS.ViewModels;

public class PositionDetailsViewModel
{
    public Position Position { get; set; }
    public Department Department { get; set; }
    public List<EmployeeBasicInfo> Employees { get; set; } = new();
    public int TotalEmployees { get; set; }
    public decimal TotalSalaryMass { get; set; }
}
