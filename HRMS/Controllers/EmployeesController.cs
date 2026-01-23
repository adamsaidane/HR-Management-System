using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly ISalaryService _salaryService;

    public EmployeesController(IEmployeeService employeeService, ISalaryService salaryService)
    {
        _employeeService = employeeService;
        _salaryService = salaryService;
    }

    // GET: Employees
    public async Task<IActionResult> Index(
        string searchString,
        int? departmentId,
        EmployeeStatus? status,
        string sortOrder,
        string currentFilter,
        int? pageNumber)
    {
        ViewBag.CurrentSort = sortOrder;
        ViewBag.MatriculeSortParam = string.IsNullOrEmpty(sortOrder) ? "matricule_desc" : "";
        ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
        ViewBag.EmailSortParam = sortOrder == "email" ? "email_desc" : "email";
        ViewBag.DepartmentSortParam = sortOrder == "department" ? "department_desc" : "department";
        ViewBag.PositionSortParam = sortOrder == "position" ? "position_desc" : "position";
        ViewBag.HireDateSortParam = sortOrder == "hiredate" ? "hiredate_desc" : "hiredate";
        ViewBag.StatusSortParam = sortOrder == "status" ? "status_desc" : "status";

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewBag.CurrentFilter = searchString;

        ViewBag.SearchString = searchString;
        ViewBag.DepartmentId = departmentId;
        ViewBag.Status = status;

        var employees = await _employeeService.GetEmployeesForIndexAsync(
            searchString, 
            departmentId, 
            status, 
            sortOrder,
            pageNumber ?? 1,
            10);

        ViewBag.Departments = await _employeeService.GetAllDepartmentsAsync();

        return View(employees);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var viewModel = await _employeeService.GetEmployeeDetailsViewModelAsync(id);
            
            viewModel.TotalBenefits = await _salaryService.GetTotalBenefitsValueAsync(id);
            viewModel.GrossSalary = await _salaryService.CalculateGrossSalaryAsync(id);
            
            return View(viewModel);
        }
        catch
        {
            return NotFound();
        }
    }

    // GET: Employees/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = await _employeeService.GetEmployeeFormViewModelAsync();
        return View(viewModel);
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Departments = await _employeeService.GetAllDepartmentsAsync();
            model.Positions = await _employeeService.GetAllPositionsAsync();
            return View(model);
        }

        try
        {
            var employee = await _employeeService.CreateEmployeeFromViewModelAsync(model);
            TempData["Success"] = "Employé créé avec succès!";
            return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.Departments = await _employeeService.GetAllDepartmentsAsync();
            model.Positions = await _employeeService.GetAllPositionsAsync();
            return View(model);
        }
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var model = await _employeeService.GetEmployeeFormViewModelAsync(id);
            return View(model);
        }
        catch
        {
            return NotFound();
        }
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Departments = await _employeeService.GetAllDepartmentsAsync();
            model.Positions = await _employeeService.GetAllPositionsAsync();
            return View(model);
        }

        try
        {
            await _employeeService.UpdateEmployeeFromViewModelAsync(id, model);
            TempData["Success"] = "Employé modifié avec succès!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.Departments = await _employeeService.GetAllDepartmentsAsync();
            model.Positions = await _employeeService.GetAllPositionsAsync();
            return View(model);
        }
    }

    // POST: Employees/UploadDocument
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(int employeeId, string documentType, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return RedirectToAction(nameof(Details), new { id = employeeId });

        try
        {
            await _employeeService.UploadEmployeeDocumentAsync(employeeId, documentType, file);
            TempData["Success"] = "Document uploadé avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erreur: " + ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = employeeId });
    }
}