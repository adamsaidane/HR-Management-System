using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByIdWithDetailsAsync(int id);
    Task<Employee?> GetByMatriculeAsync(string matricule);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status);
    Task<IQueryable<Employee>> GetQueryableWithIncludes();
    Task<string> GenerateMatriculeAsync();
}