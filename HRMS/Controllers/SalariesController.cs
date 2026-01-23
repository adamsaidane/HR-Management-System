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

    public SalariesController(ISalaryService salaryService)
    {
        _salaryService = salaryService;
    }

    // GET: Salaries/Index
    public async Task<IActionResult> Index(string searchString, int? departmentId, int? pageNumber)
    {
        var viewModel = await _salaryService.GetSalaryIndexViewModelPaginatedAsync(searchString, departmentId,
            pageNumber ?? 1,
            15);
        return View(viewModel);
    }

    // GET: Salaries/EmployeeSalary/5
    [HttpGet("Salaries/EmployeeSalary/{employeeId}")]
    public async Task<IActionResult> EmployeeSalary(int employeeId)
    {
        try
        {
            var viewModel = await _salaryService.GetEmployeeSalaryViewModelAsync(employeeId);
            return View(viewModel);
        }
        catch
        {
            return NotFound();
        }
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
        var report = await _salaryService.GetSalaryReportAsync();
        return View(report);
    }

    // GET: Salaries/DepartmentSalaries
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> DepartmentSalaries()
    {
        var report = await _salaryService.GetDepartmentSalariesReportAsync();
        return View(report);
    }
}