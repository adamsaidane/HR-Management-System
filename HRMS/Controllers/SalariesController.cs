using HRMS.Models;
using HRMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    // GET: Salaries/EmployeeSalary/5
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
}
