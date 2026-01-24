using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class DepartmentsController : Controller
{
    private readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;

    public DepartmentsController(IDepartmentService departmentService, IEmployeeService employeeService)
    {
        _departmentService = departmentService;
        _employeeService = employeeService;
    }

    // GET: Departments/Index
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        var viewModel = await _departmentService.GetDepartmentIndexViewModelAsync(
            searchString, pageNumber ?? 1, 10);
        return View(viewModel);
    }

    // GET: Departments/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var viewModel = await _departmentService.GetDepartmentDetailsAsync(id);
        if (viewModel == null)
            return NotFound();

        return View(viewModel);
    }

    // GET: Departments/PositionDetails/5
    public async Task<IActionResult> PositionDetails(int id)
    {
        var viewModel = await _departmentService.GetPositionDetailsAsync(id);
        if (viewModel == null)
            return NotFound();

        return View(viewModel);
    }

    // GET: Departments/Create
    [Authorize(Roles = "AdminRH")]
    public IActionResult Create()
    {
        var viewModel = new DepartmentFormViewModel
        {
            AvailableManagers = new List<Employee>()
        };
        return View(viewModel);
    }

    // POST: Departments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Create(DepartmentFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableManagers = new List<Employee>();
            return View(model);
        }

        try
        {
            var department = new Department
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
                ManagerId = null
            };

            await _departmentService.CreateDepartmentAsync(department);
            TempData["Success"] = "Département créé avec succès!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            model.AvailableManagers = new List<Employee>();
            return View(model);
        }
    }

    // GET: Departments/Edit/5
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
            return NotFound();

        var employees = await _employeeService.GetEmployeesByDepartmentAsync(id);
        var viewModel = new DepartmentFormViewModel
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            Code = department.Code,
            Description = department.Description,
            ManagerId = department.ManagerId,
            AvailableManagers = employees.ToList()
        };

        return View(viewModel);
    }

    // POST: Departments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Edit(int id, DepartmentFormViewModel model)
    {
        if (id != model.DepartmentId)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(id);
            model.AvailableManagers = employees.ToList();
            return View(model);
        }

        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            department.Name = model.Name;
            department.Code = model.Code;
            department.Description = model.Description;
            department.ManagerId = model.ManagerId;

            await _departmentService.UpdateDepartmentAsync(department);
            TempData["Success"] = "Département modifié avec succès!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(id);
            model.AvailableManagers = employees.ToList();
            return View(model);
        }
    }

    // GET: Departments/CreatePosition
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CreatePosition(int? departmentId)
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        var viewModel = new PositionFormViewModel
        {
            DepartmentId = departmentId ?? 0,
            Departments = departments.ToList()
        };
        return View(viewModel);
    }

    // POST: Departments/CreatePosition
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CreatePosition(PositionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            model.Departments = departments.ToList();
            return View(model);
        }

        try
        {
            var position = new Position
            {
                Title = model.Title,
                Description = model.Description,
                BaseSalary = model.BaseSalary,
                DepartmentId = model.DepartmentId
            };

            await _departmentService.CreatePositionAsync(position);
            TempData["Success"] = "Poste créé avec succès!";
            return RedirectToAction(nameof(Details), new { id = model.DepartmentId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            var departments = await _departmentService.GetAllDepartmentsAsync();
            model.Departments = departments.ToList();
            return View(model);
        }
    }

    // GET: Departments/EditPosition/5
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> EditPosition(int id)
    {
        var position = await _departmentService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound();

        var departments = await _departmentService.GetAllDepartmentsAsync();
        var viewModel = new PositionFormViewModel
        {
            PositionId = position.PositionId,
            Title = position.Title,
            Description = position.Description,
            BaseSalary = position.BaseSalary,
            DepartmentId = position.DepartmentId ?? 0,
            Departments = departments.ToList()
        };

        return View(viewModel);
    }

    // POST: Departments/EditPosition/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> EditPosition(int id, PositionFormViewModel model)
    {
        if (id != model.PositionId)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            model.Departments = departments.ToList();
            return View(model);
        }

        try
        {
            var position = await _departmentService.GetPositionByIdAsync(id);
            if (position == null)
                return NotFound();

            position.Title = model.Title;
            position.Description = model.Description;
            position.BaseSalary = model.BaseSalary;
            position.DepartmentId = model.DepartmentId;

            await _departmentService.UpdatePositionAsync(position);
            TempData["Success"] = "Poste modifié avec succès!";
            return RedirectToAction(nameof(PositionDetails), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erreur: " + ex.Message);
            var departments = await _departmentService.GetAllDepartmentsAsync();
            model.Departments = departments.ToList();
            return View(model);
        }
    }
}
