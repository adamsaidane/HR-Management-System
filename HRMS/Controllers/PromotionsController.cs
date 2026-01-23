using HRMS.Enums;
using HRMS.Service;
using HRMS.ViewModels;
using HRMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class PromotionsController : Controller
{
    private readonly IPromotionService _promotionService;
    private readonly IEmployeeService _employeeService;

    public PromotionsController(IPromotionService promotionService, IEmployeeService employeeService)
    {
        _promotionService = promotionService;
        _employeeService = employeeService;
    }

    // GET: Promotions
    public async Task<IActionResult> Index(int? pageNumber = 1)
    {
        var promotions = await _promotionService.GetAllPromotionsPaginatedAsync(pageNumber ?? 1,10);
        return View(promotions);
    }

    // GET: Promotions/Create
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Create()
    {
        var viewModel = await _promotionService.GetPromotionFormViewModelAsync();
        return View(viewModel);
    }

    // POST: Promotions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Create(PromotionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await _promotionService.GetPromotionFormViewModelAsync();
            return View(model);
        }

        try
        {
            await _promotionService.ProcessPromotionAsync(
                model.EmployeeId,
                model.NewPositionId,
                model.NewSalary,
                model.Justification);

            TempData["Success"] = "Promotion enregistrée avec succès!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            model = await _promotionService.GetPromotionFormViewModelAsync();
            return View(model);
        }
    }

    // GET: Promotions/EmployeePromotions/5
    [HttpGet("Promotions/EmployeePromotions/{employeeId}")]
    public async Task<IActionResult> EmployeePromotions(int employeeId)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        var promotions = await _promotionService.GetEmployeePromotionsAsync(employeeId);
        return View(promotions);
    }
}