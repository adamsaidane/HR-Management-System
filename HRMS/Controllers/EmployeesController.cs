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
    public IActionResult Index(string searchString, int? departmentId, EmployeeStatus? status)
    {
        var employees = _employeeService.GetAllEmployees().AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            employees = employees.Where(e =>
                e.FirstName.Contains(searchString) ||
                e.LastName.Contains(searchString) ||
                e.Matricule.Contains(searchString) ||
                e.Email.Contains(searchString));
        }

        if (departmentId.HasValue)
            employees = employees.Where(e => e.DepartmentId == departmentId);

        if (status.HasValue)
            employees = employees.Where(e => e.Status == status);

        ViewBag.Departments = _context.Departments.ToList();
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
