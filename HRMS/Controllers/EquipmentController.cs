using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;
    private readonly HRMSDbContext _context;

    public EquipmentController(IEquipmentService equipmentService, HRMSDbContext context)
    {
        _equipmentService = equipmentService;
        _context = context;
    }

    // GET: Equipment
    public IActionResult Index(string equipmentType, EquipmentStatus? status)
    {
        var equipment = _equipmentService.GetAllEquipment().AsQueryable();

        if (!string.IsNullOrEmpty(equipmentType))
            equipment = equipment.Where(e => e.EquipmentType == equipmentType);

        if (status.HasValue)
            equipment = equipment.Where(e => e.Status == status);

        ViewBag.EquipmentTypes = equipment.Select(e => e.EquipmentType).Distinct().ToList();
        return View(equipment.ToList());
    }

    // GET: Equipment/Create
    [Authorize(Roles = "AdminRH")]
    public IActionResult Create() => View();

    // POST: Equipment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult Create(Equipment equipment)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _equipmentService.CreateEquipment(equipment);
                TempData["Success"] = "Équipement créé avec succès!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur: " + ex.Message);
            }
        }
        return View(equipment);
    }

    // GET: Equipment/AssignEquipment
    [Authorize(Roles = "AdminRH")]
    public IActionResult AssignEquipment()
    {
        var viewModel = new EquipmentAssignmentViewModel
        {
            AvailableEquipment = _equipmentService.GetAvailableEquipment(),
            Employees = _context.Employees.Where(e => e.Status == EmployeeStatus.Actif).ToList()
        };
        return View(viewModel);
    }

    // POST: Equipment/AssignEquipment
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult AssignEquipment(EquipmentAssignmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _equipmentService.AssignEquipment(model.EquipmentId, model.EmployeeId, model.Condition, model.Notes);
                TempData["Success"] = "Équipement affecté avec succès!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur: " + ex.Message);
            }
        }

        model.AvailableEquipment = _equipmentService.GetAvailableEquipment();
        model.Employees = _context.Employees.Where(e => e.Status == EmployeeStatus.Actif).ToList();
        return View(model);
    }

    // POST: Equipment/ReturnEquipment
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult ReturnEquipment(int assignmentId, string condition)
    {
        try
        {
            _equipmentService.ReturnEquipment(assignmentId, DateTime.Now, condition);
            TempData["Success"] = "Équipement restitué avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Equipment/EmployeeEquipment/5
    public IActionResult EmployeeEquipment(int employeeId)
    {
        var employee = _context.Employees.Find(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        var assignments = _equipmentService.GetEmployeeEquipment(employeeId);
        return View(assignments);
    }
}
