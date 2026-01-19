using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class SalaryRepository : Repository<Salary>, ISalaryRepository
{
    public SalaryRepository(HRMSDbContext context) : base(context) { }

    public async Task<Salary?> GetCurrentSalaryAsync(int employeeId)
    {
        return await _dbSet
            .Where(s => s.EmployeeId == employeeId &&
                        (s.EndDate == null || s.EndDate > DateTime.Now))
            .OrderByDescending(s => s.EffectiveDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Salary>> GetSalaryHistoryAsync(int employeeId)
    {
        return await _dbSet
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveDate)
            .ToListAsync();
    }
}