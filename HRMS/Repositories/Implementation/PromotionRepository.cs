using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class PromotionRepository : Repository<Promotion>, IPromotionRepository
{
    public PromotionRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<Promotion>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(p => p.Employee)
            .Include(p => p.OldPosition)
            .Include(p => p.NewPosition)
            .OrderByDescending(p => p.PromotionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Promotion>> GetEmployeePromotionsAsync(int employeeId)
    {
        return await _dbSet
            .Include(p => p.OldPosition)
            .Include(p => p.NewPosition)
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.PromotionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Promotion>> GetRecentPromotionsAsync(int count)
    {
        return await _dbSet
            .Include(p => p.Employee)
            .Include(p => p.OldPosition)
            .Include(p => p.NewPosition)
            .OrderByDescending(p => p.PromotionDate)
            .Take(count)
            .ToListAsync();
    }
}