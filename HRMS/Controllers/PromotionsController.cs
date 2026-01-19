using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class PromotionsController : Controller
{
    private readonly IPromotionService _promotionService;
    private readonly ISalaryService _salaryService;
    private readonly HRMSDbContext _context;

    public PromotionsController(IPromotionService promotionService, ISalaryService salaryService, HRMSDbContext context)
    {
        _promotionService = promotionService;
        _salaryService = salaryService;
        _context = context;
    }

    // GET: Promotions
    public IActionResult Index()
    {
        var promotions = _promotionService.GetAllPromotions();
        return View(promotions);
    }

    // GET: Promotions/Create
    [Authorize(Roles = "AdminRH")]
    public IActionResult Create()
    {
        var viewModel = new PromotionFormViewModel
        {
            Employees = _context.Employees.Where(e => e.Status == EmployeeStatus.Actif).ToList(),
            Positions = _context.Positions.ToList(),
            PromotionDate = DateTime.Today
        };

        return View(viewModel);
    }

    // POST: Promotions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult Create(PromotionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors) 
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(model);
        }
        if (ModelState.IsValid)
        {
            try
            {
                _promotionService.ProcessPromotion(
                    model.EmployeeId,
                    model.NewPositionId,
                    model.NewSalary,
                    model.Justification);

                TempData["Success"] = "Promotion enregistrée avec succès!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors) 
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ModelState.AddModelError("", "Erreur: " + ex.Message);
            }
        }

        model.Employees = _context.Employees.Where(e => e.Status == EmployeeStatus.Actif).ToList();
        model.Positions = _context.Positions.ToList();
        return View(model);
    }

    // GET: Promotions/EmployeePromotions/5
    [HttpGet("Promotions/EmployeePromotions/{employeeId}")]
    public IActionResult EmployeePromotions(int employeeId)
    {
        var employee = _context.Employees.Find(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        var promotions = _promotionService.GetEmployeePromotions(employeeId);
        return View(promotions);
    }
}
