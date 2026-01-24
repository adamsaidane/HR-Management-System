using HRMS.Models;

namespace HRMS.ViewModels;

public class DepartmentIndexViewModel
{
    public PaginatedList<DepartmentListItem> Departments { get; set; }
    public string SearchString { get; set; }
    public int TotalDepartments { get; set; }
    public int TotalPositions { get; set; }
    public int TotalEmployees { get; set; }
}

public class DepartmentListItem
{
    public int DepartmentId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string ManagerName { get; set; }
    public int PositionCount { get; set; }
    public int EmployeeCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
