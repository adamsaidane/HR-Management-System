using HRMS.Enums;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(HRMSDbContext context) : base(context) { }

    public async Task<Employee?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Documents)
            .Include(e => e.Salaries)
            .Include(e => e.Bonuses)
            .Include(e => e.EmployeeBenefits)
                .ThenInclude(eb => eb.Benefit)
            .Include(e => e.EquipmentAssignments)
                .ThenInclude(ea => ea.Equipment)
            .Include(e => e.Promotions)
                .ThenInclude(p => p.OldPosition)
            .Include(e => e.Promotions)
                .ThenInclude(p => p.NewPosition)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    public async Task<Employee?> GetByMatriculeAsync(string matricule)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Matricule == matricule);
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Status == EmployeeStatus.Actif)
            .OrderBy(e => e.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
    {
        return await _dbSet
            .Include(e => e.Position)
            .Where(e => e.DepartmentId == departmentId)
            .OrderBy(e => e.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Status == status)
            .OrderBy(e => e.LastName)
            .ToListAsync();
    }

    public async Task<IQueryable<Employee>> GetQueryableWithIncludes()
    {
        return await Task.FromResult(
            _dbSet
                .Include(e => e.Department)
                .Include(e => e.Position)
        );
    }

    public async Task<string> GenerateMatriculeAsync()
    {
        var year = DateTime.Now.Year;
        var lastMatricule = await _dbSet
            .Where(e => e.Matricule.StartsWith(year.ToString()))
            .OrderByDescending(e => e.Matricule)
            .Select(e => e.Matricule)
            .FirstOrDefaultAsync();

        if (lastMatricule == null)
        {
            return $"{year}001";
        }

        var lastNumber = int.Parse(lastMatricule.Substring(4));
        var newNumber = lastNumber + 1;
        return $"{year}{newNumber:D3}";
    }
}