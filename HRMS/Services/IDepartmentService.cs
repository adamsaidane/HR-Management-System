using HRMS.Models;
using HRMS.ViewModels;

namespace HRMS.Service;

public interface IDepartmentService
{
    Task<DepartmentIndexViewModel> GetDepartmentIndexViewModelAsync(string searchString, int pageIndex = 1, int pageSize = 10);
    Task<DepartmentDetailsViewModel> GetDepartmentDetailsAsync(int departmentId);
    Task<PositionDetailsViewModel> GetPositionDetailsAsync(int positionId);
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<IEnumerable<Position>> GetPositionsByDepartmentAsync(int departmentId);
    Task<Department> GetDepartmentByIdAsync(int departmentId);
    Task<Position> GetPositionByIdAsync(int positionId);
    Task CreateDepartmentAsync(Department department);
    Task UpdateDepartmentAsync(Department department);
    Task CreatePositionAsync(Position position);
    Task UpdatePositionAsync(Position position);
    Task DeletePositionAsync(int positionId);
}
