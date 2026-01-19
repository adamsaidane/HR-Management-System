using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HRMS.Service;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public EmployeeService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _unitOfWork.Employees.GetAllAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _unitOfWork.Employees.GetByIdWithDetailsAsync(id);
    }

    public async Task<Employee?> GetEmployeeByMatriculeAsync(string matricule)
    {
        return await _unitOfWork.Employees.GetByMatriculeAsync(matricule);
    }

    public async Task CreateEmployeeAsync(Employee employee)
    {
        // Générer le matricule si nécessaire
        if (string.IsNullOrEmpty(employee.Matricule))
        {
            employee.Matricule = await _unitOfWork.Employees.GenerateMatriculeAsync();
        }

        employee.CreatedDate = DateTime.Now;
        employee.ModifiedDate = DateTime.Now;

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Récupérer la position pour le salaire initial
        var position = await _unitOfWork.Positions.GetByIdAsync(employee.PositionId);
        if (position == null)
            throw new Exception("Position introuvable");

        // Créer le premier enregistrement de salaire
        var salary = new Salary
        {
            EmployeeId = employee.EmployeeId,
            BaseSalary = position.BaseSalary,
            EffectiveDate = employee.HireDate,
            Justification = "Salaire initial à l'embauche",
            CreatedDate = DateTime.Now
        };

        await _unitOfWork.Salaries.AddAsync(salary);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        employee.ModifiedDate = DateTime.Now;
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee != null)
        {
            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        return await _unitOfWork.Employees.GetActiveEmployeesAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        return await _unitOfWork.Employees.GetByDepartmentAsync(departmentId);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status)
    {
        return await _unitOfWork.Employees.GetByStatusAsync(status);
    }

    public async Task<decimal> GetEmployeeCurrentSalaryAsync(int employeeId)
    {
        var currentSalary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employeeId);
        return currentSalary?.BaseSalary ?? 0;
    }

    public async Task<List<Employee>> GetEmployeesForIndexAsync(string searchString, int? departmentId, EmployeeStatus? status, string sortOrder)
    {
        var employees = await _unitOfWork.Employees.GetQueryableWithIncludes();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            employees = employees.Where(e =>
                e.FirstName.Contains(searchString) ||
                e.LastName.Contains(searchString) ||
                e.Matricule.Contains(searchString) ||
                e.Email.Contains(searchString));
        }

        if (departmentId.HasValue)
        {
            employees = employees.Where(e => e.DepartmentId == departmentId);
        }

        if (status.HasValue)
        {
            employees = employees.Where(e => e.Status == status);
        }

        employees = sortOrder switch
        {
            "matricule_desc" => employees.OrderByDescending(e => e.Matricule),
            "name" => employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName),
            "name_desc" => employees.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName),
            "email" => employees.OrderBy(e => e.Email),
            "email_desc" => employees.OrderByDescending(e => e.Email),
            "department" => employees.OrderBy(e => e.Department.Name),
            "department_desc" => employees.OrderByDescending(e => e.Department.Name),
            "position" => employees.OrderBy(e => e.Position.Title),
            "position_desc" => employees.OrderByDescending(e => e.Position.Title),
            "hiredate" => employees.OrderBy(e => e.HireDate),
            "hiredate_desc" => employees.OrderByDescending(e => e.HireDate),
            "status" => employees.OrderBy(e => e.Status),
            "status_desc" => employees.OrderByDescending(e => e.Status),
            _ => employees.OrderBy(e => e.Matricule)
        };

        return employees.ToList();
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        return (await _unitOfWork.Departments.GetAllAsync()).OrderBy(d => d.Name).ToList();
    }

    public async Task<List<Position>> GetAllPositionsAsync()
    {
        return (await _unitOfWork.Positions.GetAllAsync()).OrderBy(p => p.Title).ToList();
    }

    public async Task<string?> SaveEmployeePhotoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return null;
        var folder = Path.Combine(_environment.WebRootPath, "Content/Photos");
        Directory.CreateDirectory(folder);
        var fileName = Path.GetFileName(file.FileName);
        var path = Path.Combine(folder, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return "/Content/Photos/" + fileName;
    }

    public async Task UploadEmployeeDocumentAsync(int employeeId, string documentType, IFormFile file)
    {
        if (file == null || file.Length == 0) return;
        var folder = Path.Combine(_environment.WebRootPath, "Content/Documents");
        Directory.CreateDirectory(folder);
        var fileName = Path.GetFileName(file.FileName);
        var path = Path.Combine(folder, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        await _unitOfWork.Documents.AddAsync(new Document
        {
            EmployeeId = employeeId,
            DocumentType = documentType,
            FileName = fileName,
            FilePath = "/Content/Documents/" + fileName
        });
        await _unitOfWork.SaveChangesAsync();
    }
}