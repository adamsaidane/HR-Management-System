using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class SalariesController : Controller
{
    private readonly ISalaryService _salaryService;
    private readonly IUnitOfWork _unitOfWork;

    public SalariesController(ISalaryService salaryService, IUnitOfWork unitOfWork)
    {
        _salaryService = salaryService;
        _unitOfWork = unitOfWork;
    }

    // GET: Salaries/Index
    public async Task<IActionResult> Index(string searchString, int? departmentId)
    {
        var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);

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
        var allActiveEmployees = await _unitOfWork.Employees.GetActiveEmployeesAsync();
        
        decimal totalMasseSalariale = 0;
        int employeesWithSalary = 0;

        foreach (var emp in allActiveEmployees)
        {
            var currentSalary = await _salaryService.GetCurrentSalaryAsync(emp.EmployeeId);
            if (currentSalary > 0)
            {
                totalMasseSalariale += currentSalary;
                employeesWithSalary++;
            }
        }

        ViewBag.Departments = (await _unitOfWork.Departments.GetAllAsync()).OrderBy(d => d.Name).ToList();
        ViewBag.SearchString = searchString;
        ViewBag.DepartmentId = departmentId;
        ViewBag.TotalMasseSalariale = totalMasseSalariale;
        ViewBag.AverageSalary = employeesWithSalary > 0 ? totalMasseSalariale / employeesWithSalary : 0;
        ViewBag.TotalEmployees = allActiveEmployees.Count();

        return View(employees.ToList());
    }

    // GET: Salaries/EmployeeSalary/5
    [HttpGet("Salaries/EmployeeSalary/{employeeId}")]
    public async Task<IActionResult> EmployeeSalary(int employeeId)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        ViewBag.CurrentSalary = await _salaryService.GetCurrentSalaryAsync(employeeId);
        ViewBag.SalaryHistory = await _salaryService.GetSalaryHistoryAsync(employeeId);
        ViewBag.Bonuses = await _salaryService.GetBonusesByEmployeeAsync(employeeId);
        ViewBag.Benefits = await _salaryService.GetEmployeeBenefitsAsync(employeeId);
        ViewBag.TotalBenefits = await _salaryService.GetTotalBenefitsValueAsync(employeeId);
        ViewBag.AllBenefits = await _salaryService.GetAllBenefitsAsync();
        ViewBag.GrossSalary = await _salaryService.CalculateGrossSalaryAsync(employeeId);

        return View();
    }

    // POST: Salaries/UpdateSalary
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> UpdateSalary(int employeeId, decimal newSalary, string justification)
    {
        try
        {
            await _salaryService.UpdateSalaryAsync(employeeId, newSalary, justification);
            TempData["Success"] = "Salaire mis à jour avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(EmployeeSalary), new { employeeId });
    }

    // POST: Salaries/AddBonus
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> AddBonus(Bonus bonus)
    {
        try
        {
            await _salaryService.AddBonusAsync(bonus);
            TempData["Success"] = "Prime ajoutée avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(EmployeeSalary), new { employeeId = bonus.EmployeeId });
    }

    // POST: Salaries/AssignBenefit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> AssignBenefit(int employeeId, int benefitId)
    {
        try
        {
            await _salaryService.AssignBenefitToEmployeeAsync(employeeId, benefitId, DateTime.Now);
            TempData["Success"] = "Avantage assigné avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(EmployeeSalary), new { employeeId });
    }

    // GET: Salaries/SalaryReport
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> SalaryReport()
    {
        var employees = await _unitOfWork.Employees.GetActiveEmployeesAsync();

        var report = new List<object>();
        foreach (var e in employees)
        {
            report.Add(new
            {
                Employee = e,
                CurrentSalary = await _salaryService.GetCurrentSalaryAsync(e.EmployeeId),
                TotalBenefits = await _salaryService.GetTotalBenefitsValueAsync(e.EmployeeId),
                GrossSalary = await _salaryService.CalculateGrossSalaryAsync(e.EmployeeId)
            });
        }

        return View(report);
    }

    // GET: Salaries/DepartmentSalaries
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> DepartmentSalaries(int? departmentId)
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();
        var salaryByDepartment = new List<object>();

        foreach (var d in departments)
        {
            var deptEmployees = await _unitOfWork.Employees
                .FindAsync(e => e.DepartmentId == d.DepartmentId && e.Status == EmployeeStatus.Actif);

            var employeeCount = deptEmployees.Count();
            decimal totalSalary = 0;

            foreach (var emp in deptEmployees)
            {
                totalSalary += await _salaryService.GetCurrentSalaryAsync(emp.EmployeeId);
            }

            salaryByDepartment.Add(new
            {
                Department = d,
                EmployeeCount = employeeCount,
                TotalSalary = totalSalary,
                AverageSalary = employeeCount > 0 ? totalSalary / employeeCount : 0
            });
        }

        return View(salaryByDepartment);
    }
}