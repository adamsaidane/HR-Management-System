using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly HRMSDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EmployeesController(
        IEmployeeService employeeService,
        HRMSDbContext context,
        IWebHostEnvironment environment)
    {
        _employeeService = employeeService;
        _context = context;
        _environment = environment;
    }

    // GET: Employees
    public IActionResult Index(
        string searchString, 
        int? departmentId, 
        EmployeeStatus? status,
        string sortOrder,
        string currentFilter)
    {
        // Conserver les paramètres de tri
        ViewBag.CurrentSort = sortOrder;
        ViewBag.MatriculeSortParam = string.IsNullOrEmpty(sortOrder) ? "matricule_desc" : "";
        ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
        ViewBag.EmailSortParam = sortOrder == "email" ? "email_desc" : "email";
        ViewBag.DepartmentSortParam = sortOrder == "department" ? "department_desc" : "department";
        ViewBag.PositionSortParam = sortOrder == "position" ? "position_desc" : "position";
        ViewBag.HireDateSortParam = sortOrder == "hiredate" ? "hiredate_desc" : "hiredate";
        ViewBag.StatusSortParam = sortOrder == "status" ? "status_desc" : "status";

        // Reset page si nouveau filtre
        if (searchString != null)
        { 
            searchString = searchString;
        }
        else
        { 
            searchString = currentFilter;
        }

        ViewBag.CurrentFilter = searchString;

        // Récupérer tous les employés avec leurs relations
        var employees = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsQueryable();

        // Filtres
        if (!string.IsNullOrEmpty(searchString))
        {
            employees = employees.Where(e =>
                e.FirstName.Contains(searchString) ||
                e.LastName.Contains(searchString) ||
                e.Matricule.Contains(searchString) ||
                e.Email.Contains(searchString));
        }

        if (departmentId.HasValue)
        {
            employees = employees.Where(e => e.DepartmentId == departmentId);
        }

        if (status.HasValue)
        {
                employees = employees.Where(e => e.Status == status);
        }

        // Tri
        switch (sortOrder)
        {
            case "matricule_desc":
                employees = employees.OrderByDescending(e => e.Matricule);
                break;
            case "name":
                employees = employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
                break;
            case "name_desc":
                employees = employees.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName);
                break;
            case "email":
                employees = employees.OrderBy(e => e.Email);
                break;
            case "email_desc":
                employees = employees.OrderByDescending(e => e.Email);
                break;
            case "department":
                employees = employees.OrderBy(e => e.Department.Name);
                break;
            case "department_desc":
                employees = employees.OrderByDescending(e => e.Department.Name);
                break;
            case "position":
                employees = employees.OrderBy(e => e.Position.Title);
                break;
            case "position_desc":
                employees = employees.OrderByDescending(e => e.Position.Title);
                break;
            case "hiredate":
                employees = employees.OrderBy(e => e.HireDate);
                break;
            case "hiredate_desc":
                employees = employees.OrderByDescending(e => e.HireDate);
                break;
            case "status":
                employees = employees.OrderBy(e => e.Status);
                break;
            case "status_desc":
                employees = employees.OrderByDescending(e => e.Status);
                break; 
            default:
                employees = employees.OrderBy(e => e.Matricule); 
                break;
        }

        // Données pour les filtres
        ViewBag.Departments = _context.Departments.OrderBy(d => d.Name).ToList();
        ViewBag.SearchString = searchString; 
        ViewBag.DepartmentId = departmentId;
        ViewBag.Status = status;
            
        return View(employees.ToList());
    }

    // GET: Employees/Details/5
    public IActionResult Details(int id)
    {
        var employee = _employeeService.GetEmployeeById(id);
        if (employee == null)
            return NotFound();

        var salaryService = new SalaryService(_context);

        var viewModel = new EmployeeDetailsViewModel
        {
            Employee = employee,
            CurrentSalary = salaryService.GetCurrentSalary(id),
            TotalBenefits = salaryService.GetTotalBenefitsValue(id),
            GrossSalary = salaryService.CalculateGrossSalary(id),
            SalaryHistory = salaryService.GetSalaryHistory(id).ToList(),
            Bonuses = salaryService.GetBonusesByEmployee(id).ToList(),
            Benefits = salaryService.GetEmployeeBenefits(id).ToList(),
            Equipments = employee.EquipmentAssignments.ToList(),
            Promotions = employee.Promotions.ToList(),
            Documents = employee.Documents.ToList()
        };

        return View(viewModel);
    }

    // GET: Employees/Create
    public IActionResult Create()
    {
        return View(new EmployeeFormViewModel
        {
            Departments = _context.Departments.ToList(),
            Positions = _context.Positions.ToList(),
            HireDate = DateTime.Today
        });
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EmployeeFormViewModel model)
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
            var folder = Path.Combine(_environment.WebRootPath, "Content/Photos");
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(model.Photo.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            model.Photo.CopyTo(stream);

            employee.PhotoPath = "/Content/Photos/" + fileName;
        }

        _employeeService.CreateEmployee(employee);

        TempData["Success"] = "Employé créé avec succès!";
        return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
    }

    // GET: Employees/Edit/5
    public IActionResult Edit(int id)
    {
        var employee = _employeeService.GetEmployeeById(id);
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
            Departments = _context.Departments.ToList(),
            Positions = _context.Positions.ToList()
        };

        return View(model);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Departments = _context.Departments.ToList();
            model.Positions = _context.Positions.ToList();
            return View(model);
        }

        var employee = _employeeService.GetEmployeeById(id);
        if (employee == null)
            return NotFound();

        // Mettre à jour les champs éditables
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

        // Gestion de la photo (optionnelle)
        if (model.Photo != null && model.Photo.Length > 0)
        {
            var folder = Path.Combine(_environment.WebRootPath, "Content/Photos");
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(model.Photo.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            model.Photo.CopyTo(stream);

            employee.PhotoPath = "/Content/Photos/" + fileName;
        }

        _employeeService.UpdateEmployee(employee);
        TempData["Success"] = "Employé modifié avec succès!";
        return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
    }

    // POST: Employees/UploadDocument
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadDocument(int employeeId, string documentType, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return RedirectToAction(nameof(Details), new { id = employeeId });

        var folder = Path.Combine(_environment.WebRootPath, "Content/Documents");
        Directory.CreateDirectory(folder);

        var fileName = Path.GetFileName(file.FileName);
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        file.CopyTo(stream);

        _context.Documents.Add(new Document
        {
            EmployeeId = employeeId,
            DocumentType = documentType,
            FileName = fileName,
            FilePath = "/Content/Documents/" + fileName
        });

        _context.SaveChanges();
        TempData["Success"] = "Document uploadé avec succès!";

        return RedirectToAction(nameof(Details), new { id = employeeId });
    }
}
