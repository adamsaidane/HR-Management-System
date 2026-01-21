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
    private readonly IEmployeeService _employeeService;

    public EquipmentController(IEquipmentService equipmentService, IEmployeeService employeeService)
    {
        _equipmentService = equipmentService;
        _employeeService = employeeService;
    }

    // GET: Equipment
    public async Task<IActionResult> Index(string equipmentType, EquipmentStatus? status)
    {
        var viewModel = await _equipmentService.GetEquipmentIndexViewModelAsync(equipmentType, status);
        return View(viewModel);
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
        if (!ModelState.IsValid)
            return View(equipment);

        try
        {
            await _equipmentService.CreateEquipmentAsync(equipment);
            TempData["Success"] = "Équipement créé avec succès!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            return View(equipment);
        }
    }
    
    // GET: Equipment/Edit/5
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Edit(int id)
    {
        var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
        if (equipment == null)
            return NotFound();

        return View(equipment);
    }
    
    // POST: Equipment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Edit(int id, Equipment equipment)
    {
        if (!ModelState.IsValid)
            return View(equipment);

        try
        {
            await _equipmentService.UpdateEquipmentAsync(equipment);
            TempData["Success"] = "Équipement modifié avec succès!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            return View(equipment);
        }
    }

    // GET: Equipment/AssignEquipment
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> AssignEquipment()
    {
        var viewModel = await _equipmentService.GetEquipmentAssignmentViewModelAsync();
        return View(viewModel);
    }

    // POST: Equipment/AssignEquipment
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> AssignEquipment(EquipmentAssignmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await _equipmentService.GetEquipmentAssignmentViewModelAsync();
            return View(model);
        }

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
            model = await _equipmentService.GetEquipmentAssignmentViewModelAsync();
            return View(model);
        }
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
        var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
            return NotFound();

        ViewBag.Employee = employee;
        var assignments = await _equipmentService.GetEmployeeEquipmentAsync(employeeId);
        return View(assignments);
    }
}