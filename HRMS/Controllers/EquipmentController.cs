using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using HRMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentController(IEquipmentService equipmentService, IUnitOfWork unitOfWork)
    {
        _equipmentService = equipmentService;
        _unitOfWork = unitOfWork;
    }

    // GET: Equipment
    public async Task<IActionResult> Index(string equipmentType, EquipmentStatus? status)
    {
        var equipment = await _equipmentService.GetAllEquipmentAsync();

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
    public async Task<IActionResult> Create(Equipment equipment)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _equipmentService.CreateEquipmentAsync(equipment);
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
    public async Task<IActionResult> AssignEquipment()
    {
        var viewModel = new EquipmentAssignmentViewModel
        {
            AvailableEquipment = (await _equipmentService.GetAvailableEquipmentAsync()).ToList(),
            Employees = (await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif)).ToList()
        };
        return View(viewModel);
    }

    // POST: Equipment/AssignEquipment
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> AssignEquipment(EquipmentAssignmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _equipmentService.AssignEquipmentAsync(
                    model.EquipmentId, 
                    model.EmployeeId, 
                    model.Condition, 
                    model.Notes);
                TempData["Success"] = "Équipement affecté avec succès!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur: " + ex.Message);
            }
        }

        model.AvailableEquipment = (await _equipmentService.GetAvailableEquipmentAsync()).ToList();
        model.Employees = (await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif)).ToList();
        return View(model);
    }

    // POST: Equipment/ReturnEquipment
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> ReturnEquipment(int assignmentId, string condition)
    {
        try
        {
            await _equipmentService.ReturnEquipmentAsync(assignmentId, DateTime.Now, condition);
            TempData["Success"] = "Équipement restitué avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Equipment/EmployeeEquipment/5
    [HttpGet("Equipment/EmployeeEquipment/{employeeId}")]
    public async Task<IActionResult> EmployeeEquipment(int employeeId)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        var assignments = await _equipmentService.GetEmployeeEquipmentAsync(employeeId);
        return View(assignments);
    }
}