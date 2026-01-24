using Microsoft.EntityFrameworkCore;
using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;

namespace HRMS.Service;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentIndexViewModel> GetDepartmentIndexViewModelAsync(string searchString, int pageIndex = 1, int pageSize = 10)
    {
        var departmentsQuery = _unitOfWork.Departments.GetAllQueryable()
            .Include(d => d.Manager)
            .Include(d => d.Positions)
            .Include(d => d.Employees)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.ToLower();
            departmentsQuery = departmentsQuery.Where(d =>
                d.Name.ToLower().Contains(searchString) ||
                d.Code.ToLower().Contains(searchString));
        }

        var totalDepartments = await departmentsQuery.CountAsync();
        var allDepartments = await _unitOfWork.Departments.GetAllQueryable()
            .Include(d => d.Positions)
            .Include(d => d.Employees)
            .ToListAsync();

        var departmentItems = departmentsQuery
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentListItem
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                ManagerName = d.Manager != null ? d.Manager.FirstName + " " + d.Manager.LastName : "Non assigné",
                PositionCount = d.Positions.Count,
                EmployeeCount = d.Employees.Count(e => e.Status == EmployeeStatus.Actif),
                CreatedDate = d.CreatedDate
            });

        var paginatedDepartments = await PaginatedList<DepartmentListItem>.CreateAsync(
            departmentItems, pageIndex, pageSize);

        return new DepartmentIndexViewModel
        {
            Departments = paginatedDepartments,
            SearchString = searchString,
            TotalDepartments = totalDepartments,
            TotalPositions = allDepartments.Sum(d => d.Positions.Count),
            TotalEmployees = allDepartments.Sum(d => d.Employees.Count(e => e.Status == EmployeeStatus.Actif))
        };
    }

    public async Task<DepartmentDetailsViewModel> GetDepartmentDetailsAsync(int departmentId)
    {
        var department = await _unitOfWork.Departments.GetAllQueryable()
            .Include(d => d.Manager)
            .Include(d => d.Positions)
                .ThenInclude(p => p.Employees.Where(e => e.Status == EmployeeStatus.Actif))
            .Include(d => d.Employees.Where(e => e.Status == EmployeeStatus.Actif))
                .ThenInclude(e => e.Salaries)
            .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

        if (department == null)
            return null;

        var positions = department.Positions.Select(p => new PositionWithEmployees
        {
            PositionId = p.PositionId,
            Title = p.Title,
            Description = p.Description,
            BaseSalary = p.BaseSalary,
            EmployeeCount = p.Employees.Count(e => e.Status == EmployeeStatus.Actif),
            Employees = p.Employees
                .Where(e => e.Status == EmployeeStatus.Actif)
                .Select(e => new EmployeeBasicInfo
                {
                    EmployeeId = e.EmployeeId,
                    Matricule = e.Matricule,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    PhotoPath = e.PhotoPath,
                    HireDate = e.HireDate,
                    Status = e.Status.ToString()
                }).ToList()
        }).OrderBy(p => p.Title).ToList();

        var activeEmployees = department.Employees.Where(e => e.Status == EmployeeStatus.Actif).ToList();
        var totalSalary = activeEmployees.Sum(e =>
            e.Salaries.Where(s => s.EndDate == null || s.EndDate > DateTime.Now)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefault()?.BaseSalary ?? 0);

        return new DepartmentDetailsViewModel
        {
            Department = department,
            Positions = positions,
            TotalEmployees = activeEmployees.Count,
            TotalSalaryMass = totalSalary,
            AverageSalary = activeEmployees.Count > 0 ? totalSalary / activeEmployees.Count : 0
        };
    }

    public async Task<PositionDetailsViewModel> GetPositionDetailsAsync(int positionId)
    {
        var position = await _unitOfWork.Positions.GetAllQueryable()
            .Include(p => p.Department)
            .Include(p => p.Employees.Where(e => e.Status == EmployeeStatus.Actif))
                .ThenInclude(e => e.Salaries)
            .FirstOrDefaultAsync(p => p.PositionId == positionId);

        if (position == null)
            return null;

        var employees = position.Employees
            .Where(e => e.Status == EmployeeStatus.Actif)
            .Select(e => new EmployeeBasicInfo
            {
                EmployeeId = e.EmployeeId,
                Matricule = e.Matricule,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                PhotoPath = e.PhotoPath,
                HireDate = e.HireDate,
                Status = e.Status.ToString()
            }).ToList();

        var totalSalary = position.Employees
            .Where(e => e.Status == EmployeeStatus.Actif)
            .Sum(e => e.Salaries
                .Where(s => s.EndDate == null || s.EndDate > DateTime.Now)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefault()?.BaseSalary ?? 0);

        return new PositionDetailsViewModel
        {
            Position = position,
            Department = position.Department,
            Employees = employees,
            TotalEmployees = employees.Count,
            TotalSalaryMass = totalSalary
        };
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _unitOfWork.Departments.GetAllQueryable()
            .Include(d => d.Manager)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Position>> GetPositionsByDepartmentAsync(int departmentId)
    {
        return await _unitOfWork.Positions.GetAllQueryable()
            .Where(p => p.DepartmentId == departmentId)
            .Include(p => p.Employees)
            .OrderBy(p => p.Title)
            .ToListAsync();
    }

    public async Task<Department> GetDepartmentByIdAsync(int departmentId)
    {
        return await _unitOfWork.Departments.GetAllQueryable()
            .Include(d => d.Manager)
            .Include(d => d.Positions)
            .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);
    }

    public async Task<Position> GetPositionByIdAsync(int positionId)
    {
        return await _unitOfWork.Positions.GetAllQueryable()
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.PositionId == positionId);
    }

    public async Task CreateDepartmentAsync(Department department)
    {
        department.CreatedDate = DateTime.Now;
        department.ModifiedDate = DateTime.Now;
        await _unitOfWork.Departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateDepartmentAsync(Department department)
    {
        department.ModifiedDate = DateTime.Now;
        _unitOfWork.Departments.Update(department);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CreatePositionAsync(Position position)
    {
        position.CreatedDate = DateTime.Now;
        position.ModifiedDate = DateTime.Now;
        await _unitOfWork.Positions.AddAsync(position);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdatePositionAsync(Position position)
    {
        position.ModifiedDate = DateTime.Now;
        _unitOfWork.Positions.Update(position);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeletePositionAsync(int positionId)
    {
        var position = await _unitOfWork.Positions.GetByIdAsync(positionId);
        if (position != null)
        {
            _unitOfWork.Positions.Delete(position);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
