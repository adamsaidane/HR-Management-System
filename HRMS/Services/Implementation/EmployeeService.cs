using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;

namespace HRMS.Service;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
}