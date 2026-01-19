using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class BonusRepository : Repository<Bonus>, IBonusRepository
{
    public BonusRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<Bonus>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Where(b => b.EmployeeId == employeeId)
            .OrderByDescending(b => b.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalBonusesAsync(int employeeId, int year)
    {
        return await _dbSet
            .Where(b => b.EmployeeId == employeeId && b.Date.Year == year)
            .SumAsync(b => (decimal?)b.Amount) ?? 0;
    }
}
