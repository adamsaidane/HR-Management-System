using HRMS.Enums;
using HRMS.Models;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Http;

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

    // Abstractions for EmployeesController
    Task<List<Employee>> GetEmployeesForIndexAsync(string searchString, int? departmentId, EmployeeStatus? status, string sortOrder);
    Task<List<Department>> GetAllDepartmentsAsync();
    Task<List<Position>> GetAllPositionsAsync();
    Task<string?> SaveEmployeePhotoAsync(IFormFile file);
    Task UploadEmployeeDocumentAsync(int employeeId, string documentType, IFormFile file);
    Task<EmployeeDetailsViewModel> GetEmployeeDetailsViewModelAsync(int employeeId);
    Task<EmployeeFormViewModel> GetEmployeeFormViewModelAsync(int? employeeId = null);
    Task<Employee> CreateEmployeeFromViewModelAsync(EmployeeFormViewModel model);
    Task UpdateEmployeeFromViewModelAsync(int id, EmployeeFormViewModel model);
    Task<PaginatedList<Employee>> GetEmployeesPaginatedForIndexAsync(
        string searchString, 
        int? departmentId, 
        EmployeeStatus? status, 
        string sortOrder,
        int pageIndex = 1,
        int pageSize = 10);
}