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

    public EmployeesController(
        IEmployeeService employeeService,
        ISalaryService salaryService)
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
        string currentFilter)
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
            searchString = searchString;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewBag.CurrentFilter = searchString;

        var employees = await _employeeService.GetEmployeesForIndexAsync(searchString, departmentId, status, sortOrder);

        ViewBag.Departments = await _employeeService.GetAllDepartmentsAsync();
        ViewBag.SearchString = searchString;
        ViewBag.DepartmentId = departmentId;
        ViewBag.Status = status;

        return View(employees);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        var viewModel = new EmployeeDetailsViewModel
        {
            Employee = employee,
            CurrentSalary = await _salaryService.GetCurrentSalaryAsync(id),
            TotalBenefits = await _salaryService.GetTotalBenefitsValueAsync(id),
            GrossSalary = await _salaryService.CalculateGrossSalaryAsync(id),
            SalaryHistory = (await _salaryService.GetSalaryHistoryAsync(id)).ToList(),
            Bonuses = (await _salaryService.GetBonusesByEmployeeAsync(id)).ToList(),
            Benefits = (await _salaryService.GetEmployeeBenefitsAsync(id)).ToList(),
            Equipments = employee.EquipmentAssignments.ToList(),
            Promotions = employee.Promotions.ToList(),
            Documents = employee.Documents.ToList()
        };

        return View(viewModel);
    }

    // GET: Employees/Create
    public async Task<IActionResult> Create()
    {
        return View(new EmployeeFormViewModel
        {
            Departments = await _employeeService.GetAllDepartmentsAsync(),
            Positions = await _employeeService.GetAllPositionsAsync(),
            HireDate = DateTime.Today
        });
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

        var employee = new Employee
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Address = model.Address,
            Phone = model.Phone,
            Email = model.Email,
            DepartmentId = model.DepartmentId.Value,
            PositionId = model.PositionId.Value,
            HireDate = model.HireDate,
            ContractType = model.ContractType,
            Status = EmployeeStatus.Actif
        };

        if (model.Photo != null && model.Photo.Length > 0)
        {
            var savedPath = await _employeeService.SaveEmployeePhotoAsync(model.Photo);
            if (!string.IsNullOrEmpty(savedPath))
            {
                employee.PhotoPath = savedPath;
            }
        }

        await _employeeService.CreateEmployeeAsync(employee);

        TempData["Success"] = "Employé créé avec succès!";
        return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        var model = new EmployeeFormViewModel
        {
            EmployeeId = employee.EmployeeId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DateOfBirth = employee.DateOfBirth,
            Address = employee.Address,
            Phone = employee.Phone,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            PositionId = employee.PositionId,
            HireDate = employee.HireDate,
            ContractType = employee.ContractType,
            Departments = await _employeeService.GetAllDepartmentsAsync(),
            Positions = await _employeeService.GetAllPositionsAsync()
        };

        return View(model);
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

        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        employee.FirstName = model.FirstName;
        employee.LastName = model.LastName;
        employee.DateOfBirth = model.DateOfBirth;
        employee.Address = model.Address;
        employee.Phone = model.Phone;
        employee.Email = model.Email;
        employee.DepartmentId = model.DepartmentId!.Value;
        employee.PositionId = model.PositionId!.Value;
        employee.HireDate = model.HireDate;
        employee.ContractType = model.ContractType;

        if (model.Photo != null && model.Photo.Length > 0)
        {
            var savedPath = await _employeeService.SaveEmployeePhotoAsync(model.Photo);
            if (!string.IsNullOrEmpty(savedPath))
            {
                employee.PhotoPath = savedPath;
            }
        }

        await _employeeService.UpdateEmployeeAsync(employee);
        TempData["Success"] = "Employé modifié avec succès!";
        return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
    }

    // POST: Employees/UploadDocument
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(int employeeId, string documentType, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return RedirectToAction(nameof(Details), new { id = employeeId });

        await _employeeService.UploadEmployeeDocumentAsync(employeeId, documentType, file);
        TempData["Success"] = "Document uploadé avec succès!";

        return RedirectToAction(nameof(Details), new { id = employeeId });
    }
}