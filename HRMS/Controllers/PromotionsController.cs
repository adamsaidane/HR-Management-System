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
    private readonly IUnitOfWork _unitOfWork;

    public PromotionsController(IPromotionService promotionService, IUnitOfWork unitOfWork)
    {
        _promotionService = promotionService;
        _unitOfWork = unitOfWork;
    }

    // GET: Promotions
    public async Task<IActionResult> Index()
    {
        var promotions = await _promotionService.GetAllPromotionsAsync();
        return View(promotions);
    }

    // GET: Promotions/Create
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Create()
    {
        var viewModel = new PromotionFormViewModel
        {
            Employees = (await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif)).ToList(),
            Positions = (await _unitOfWork.Positions.GetAllAsync()).ToList(),
            PromotionDate = DateTime.Today
        };

        return View(viewModel);
    }

    // POST: Promotions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Create(PromotionFormViewModel model)
    {
        if (ModelState.IsValid)
        {
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
            }
        }

        model.Employees = (await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif)).ToList();
        model.Positions = (await _unitOfWork.Positions.GetAllAsync()).ToList();
        return View(model);
    }

    // GET: Promotions/EmployeePromotions/5
    [HttpGet("Promotions/EmployeePromotions/{employeeId}")]
    public async Task<IActionResult> EmployeePromotions(int employeeId)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        var promotions = await _promotionService.GetEmployeePromotionsAsync(employeeId);
        return View(promotions);
    }
}