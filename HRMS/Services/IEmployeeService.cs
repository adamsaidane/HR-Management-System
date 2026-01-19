using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Service;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<Employee?> GetEmployeeByMatriculeAsync(string matricule);
    Task CreateEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int id);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status);
    Task<decimal> GetEmployeeCurrentSalaryAsync(int employeeId);
}