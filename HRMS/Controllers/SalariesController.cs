using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Controllers;

[Authorize]
public class SalariesController : Controller
{
    private readonly ISalaryService _salaryService;
    private readonly HRMSDbContext _context;

    public SalariesController(ISalaryService salaryService, HRMSDbContext context)
    {
        _salaryService = salaryService;
        _context = context;
    }

    // GET: Salaries/Index
    // Liste tous les employés actifs avec leurs salaires
    public IActionResult Index(string searchString, int? departmentId)
    {
        // Récupérer tous les employés actifs
        var employees = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Salaries)
            .Where(e => e.Status == EmployeeStatus.Actif)
            .AsQueryable();

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

        // Trier par nom
        employees = employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);

        // Données pour les filtres
        ViewBag.Departments = _context.Departments.OrderBy(d => d.Name).ToList();
        ViewBag.SearchString = searchString;
        ViewBag.DepartmentId = departmentId;

        // Statistiques globales
        var allActiveEmployees = _context.Employees
            .Include(e => e.Salaries)
            .Where(e => e.Status == EmployeeStatus.Actif)
            .ToList();

        decimal totalMasseSalariale = 0;
        int employeesWithSalary = 0;

        foreach (var emp in allActiveEmployees)
        {
            var currentSalary = emp.Salaries
                .Where(s => s.EndDate == null || s.EndDate > DateTime.Now)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefault();

            if (currentSalary != null)
            {
                totalMasseSalariale += currentSalary.BaseSalary;
                employeesWithSalary++;
            }
        }

        ViewBag.TotalMasseSalariale = totalMasseSalariale;
        ViewBag.AverageSalary = employeesWithSalary > 0 ? totalMasseSalariale / employeesWithSalary : 0;
        ViewBag.TotalEmployees = allActiveEmployees.Count;

        return View(employees.ToList());
    }
        
    // GET: Salaries/EmployeeSalary/5
    [HttpGet("Salaries/EmployeeSalary/{employeeId}")]
    public IActionResult EmployeeSalary(int employeeId)
    {
        var employee = _context.Employees.Find(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        ViewBag.CurrentSalary = _salaryService.GetCurrentSalary(employeeId);
        ViewBag.SalaryHistory = _salaryService.GetSalaryHistory(employeeId);
        ViewBag.Bonuses = _salaryService.GetBonusesByEmployee(employeeId);
        ViewBag.Benefits = _salaryService.GetEmployeeBenefits(employeeId);
        ViewBag.TotalBenefits = _salaryService.GetTotalBenefitsValue(employeeId);
        ViewBag.GrossSalary = _salaryService.CalculateGrossSalary(employeeId);

        return View();
    }

    // POST: Salaries/UpdateSalary
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult UpdateSalary(int employeeId, decimal newSalary, string justification)
    {
        try
        {
            _salaryService.UpdateSalary(employeeId, newSalary, justification);
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
    public IActionResult AddBonus(Bonus bonus)
    {
        try
        {
            _salaryService.AddBonus(bonus);
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
    public IActionResult AssignBenefit(int employeeId, int benefitId)
    {
        try
        {
            _salaryService.AssignBenefitToEmployee(employeeId, benefitId, DateTime.Now);
            TempData["Success"] = "Avantage assigné avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(EmployeeSalary), new { employeeId });
    }
    [Authorize(Roles = "AdminRH")]
    public IActionResult SalaryReport()
    {
        var employees = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Salaries)
            .Where(e => e.Status == EmployeeStatus.Actif)
            .ToList();

        var report = employees.Select(e => new
        {
            Employee = e,
            CurrentSalary = _salaryService.GetCurrentSalary(e.EmployeeId),
            TotalBenefits = _salaryService.GetTotalBenefitsValue(e.EmployeeId),
            GrossSalary = _salaryService.CalculateGrossSalary(e.EmployeeId)
        }).ToList();

        return View(report);
    }

    // GET: Salaries/DepartmentSalaries
    // Salaires par département
    [Authorize(Roles = "AdminRH,Manager")]
    public IActionResult DepartmentSalaries(int? departmentId)
    {
        var departments = _context.Departments
            .Include(d => d.Employees.Where(e => e.Status == EmployeeStatus.Actif))
            .ThenInclude(e => e.Salaries)
            .ToList();

        var salaryByDepartment = departments.Select(d => new
        {
            Department = d,
            EmployeeCount = d.Employees.Count(e => e.Status == EmployeeStatus.Actif),
            TotalSalary = d.Employees
                .Where(e => e.Status == EmployeeStatus.Actif)
                .Sum(e => _salaryService.GetCurrentSalary(e.EmployeeId)),
            AverageSalary = d.Employees.Count(e => e.Status == EmployeeStatus.Actif) > 0
                ? d.Employees
                    .Where(e => e.Status == EmployeeStatus.Actif)
                    .Average(e => _salaryService.GetCurrentSalary(e.EmployeeId))
                : 0
        }).ToList();

        return View(salaryByDepartment);
    }
}
