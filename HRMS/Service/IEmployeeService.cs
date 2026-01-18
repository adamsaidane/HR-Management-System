using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Service;

public interface IEmployeeService
{
    IEnumerable<Employee> GetAllEmployees();
    Employee GetEmployeeById(int id);
    Employee GetEmployeeByMatricule(string matricule);
    void CreateEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(int id);
        
    IEnumerable<Employee> GetActiveEmployees();
    IEnumerable<Employee> GetEmployeesByDepartment(int departmentId);
    IEnumerable<Employee> GetEmployeesByStatus(EmployeeStatus status);
    decimal GetEmployeeCurrentSalary(int employeeId);
    string GenerateMatricule();
}
  