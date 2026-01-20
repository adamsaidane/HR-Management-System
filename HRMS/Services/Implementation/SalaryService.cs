using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class SalaryService : ISalaryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeService _employeeService;

    public SalaryService(IUnitOfWork unitOfWork, IEmployeeService employeeService)
    {
        _unitOfWork = unitOfWork;
        _employeeService = employeeService;
    }
    public async Task<decimal> GetCurrentSalaryAsync(int employeeId)
    {
        var currentSalary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employeeId);
        return currentSalary?.BaseSalary ?? 0;
    }

    public async Task<IEnumerable<Salary>> GetSalaryHistoryAsync(int employeeId)
    {
        return await _unitOfWork.Salaries.GetSalaryHistoryAsync(employeeId);
    }

    public async Task AddSalaryAsync(Salary salary)
    {
        salary.CreatedDate = DateTime.Now;
        await _unitOfWork.Salaries.AddAsync(salary);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateSalaryAsync(int employeeId, decimal newSalary, string justification)
    {
        // Clôturer l'ancien salaire
        var currentSalary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employeeId);

        if (currentSalary != null)
        {
            currentSalary.EndDate = DateTime.Now.AddDays(-1);
            _unitOfWork.Salaries.Update(currentSalary);
        }

        // Créer le nouveau salaire
        var newSalaryRecord = new Salary
        {
            EmployeeId = employeeId,
            BaseSalary = newSalary,
            EffectiveDate = DateTime.Now,
            Justification = justification,
            CreatedDate = DateTime.Now
        };

        await _unitOfWork.Salaries.AddAsync(newSalaryRecord);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Bonus>> GetBonusesByEmployeeAsync(int employeeId)
    {
        return await _unitOfWork.Bonuses.GetByEmployeeAsync(employeeId);
    }

    public async Task AddBonusAsync(Bonus bonus)
    {
        bonus.CreatedDate = DateTime.Now;
        await _unitOfWork.Bonuses.AddAsync(bonus);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalBonusesAsync(int employeeId, int year)
    {
        return await _unitOfWork.Bonuses.GetTotalBonusesAsync(employeeId, year);
    }

    public async Task<IEnumerable<Benefit>> GetAllBenefitsAsync()
    {
        return await _unitOfWork.Benefits.GetAllAsync();
    }

    public async Task<IEnumerable<EmployeeBenefit>> GetEmployeeBenefitsAsync(int employeeId)
    {
        return await _unitOfWork.EmployeeBenefits.GetEmployeeBenefitsAsync(employeeId);
    }

    public async Task AssignBenefitToEmployeeAsync(int employeeId, int benefitId, DateTime startDate)
    {
        var employeeBenefit = new EmployeeBenefit
        {
            EmployeeId = employeeId,
            BenefitId = benefitId,
            StartDate = startDate
        };

        await _unitOfWork.EmployeeBenefits.AddAsync(employeeBenefit);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveBenefitFromEmployeeAsync(int employeeBenefitId, DateTime endDate)
    {
        var employeeBenefit = await _unitOfWork.EmployeeBenefits.GetByIdAsync(employeeBenefitId);
        if (employeeBenefit != null)
        {
            employeeBenefit.EndDate = endDate;
            _unitOfWork.EmployeeBenefits.Update(employeeBenefit);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalBenefitsValueAsync(int employeeId)
    {
        return await _unitOfWork.EmployeeBenefits.GetTotalBenefitsValueAsync(employeeId);
    }

    public async Task<decimal> CalculateGrossSalaryAsync(int employeeId)
    {
        var baseSalary = await GetCurrentSalaryAsync(employeeId);
        var benefits = await GetTotalBenefitsValueAsync(employeeId);
        return baseSalary + benefits;
    }

    public async Task<decimal> CalculateTotalCompensationAsync(int employeeId, int year)
    {
        var baseSalary = await GetCurrentSalaryAsync(employeeId) * 12;
        var bonuses = await GetTotalBonusesAsync(employeeId, year);
        var benefits = await GetTotalBenefitsValueAsync(employeeId) * 12;
        return baseSalary + bonuses + benefits;
    }
    public async Task<SalaryIndexViewModel> GetSalaryIndexViewModelAsync(string searchString, int? departmentId)
    {
        var employees = await _employeeService.GetEmployeesByStatusAsync(EmployeeStatus.Actif);

        // Filtre par recherche
        if (!string.IsNullOrEmpty(searchString))
        {
            employees = employees.Where(e =>
                e.FirstName.Contains(searchString) ||
                e.LastName.Contains(searchString) ||
                e.Matricule.Contains(searchString));
        }

        // Filtre par département
        if (departmentId.HasValue)
        {
            employees = employees.Where(e => e.DepartmentId == departmentId.Value);
        }

        employees = employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);

        // Statistiques globales
        var allActiveEmployees = await _employeeService.GetActiveEmployeesAsync();
        
        decimal totalMasseSalariale = 0;
        int employeesWithSalary = 0;

        foreach (var emp in allActiveEmployees)
        {
            var currentSalary = await GetCurrentSalaryAsync(emp.EmployeeId);
            if (currentSalary > 0)
            {
                totalMasseSalariale += currentSalary;
                employeesWithSalary++;
            }
        }

        return new SalaryIndexViewModel
        {
            Employees = employees.ToList(),
            Departments = await _employeeService.GetAllDepartmentsAsync(),
            SearchString = searchString,
            DepartmentId = departmentId,
            TotalMasseSalariale = totalMasseSalariale,
            AverageSalary = employeesWithSalary > 0 ? totalMasseSalariale / employeesWithSalary : 0,
            TotalEmployees = allActiveEmployees.Count()
        };
    }

    public async Task<EmployeeSalaryViewModel> GetEmployeeSalaryViewModelAsync(int employeeId)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
            throw new Exception("Employé introuvable");

        return new EmployeeSalaryViewModel
        {
            Employee = employee,
            CurrentSalary = await GetCurrentSalaryAsync(employeeId),
            SalaryHistory = (await GetSalaryHistoryAsync(employeeId)).ToList(),
            Bonuses = (await GetBonusesByEmployeeAsync(employeeId)).ToList(),
            Benefits = (await GetEmployeeBenefitsAsync(employeeId)).ToList(),
            AllBenefits = (await GetAllBenefitsAsync()).ToList(),
            TotalBenefits = await GetTotalBenefitsValueAsync(employeeId),
            GrossSalary = await CalculateGrossSalaryAsync(employeeId)
        };
    }

    public async Task<List<SalaryReportItem>> GetSalaryReportAsync()
    {
        var employees = await _employeeService.GetActiveEmployeesAsync();

        var report = new List<SalaryReportItem>();
        foreach (var e in employees)
        {
            report.Add(new SalaryReportItem
            {
                Employee = e,
                CurrentSalary = await GetCurrentSalaryAsync(e.EmployeeId),
                TotalBenefits = await GetTotalBenefitsValueAsync(e.EmployeeId),
                GrossSalary = await CalculateGrossSalaryAsync(e.EmployeeId)
            });
        }

        return report;
    }

    public async Task<List<DepartmentSalaryReport>> GetDepartmentSalariesReportAsync()
    {
        var departments = await _employeeService.GetAllDepartmentsAsync();
        var report = new List<DepartmentSalaryReport>();

        foreach (var d in departments)
        {
            var deptEmployees = await _employeeService.GetEmployeesByDepartmentAsync(d.DepartmentId);
            var activeEmployees = deptEmployees.Where(e => e.Status == EmployeeStatus.Actif);

            var employeeCount = activeEmployees.Count();
            decimal totalSalary = 0;

            foreach (var emp in activeEmployees)
            {
                totalSalary += await GetCurrentSalaryAsync(emp.EmployeeId);
            }

            report.Add(new DepartmentSalaryReport
            {
                Department = d,
                EmployeeCount = employeeCount,
                TotalSalary = totalSalary,
                AverageSalary = employeeCount > 0 ? totalSalary / employeeCount : 0
            });
        }

        return report;
    }
}