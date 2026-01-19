using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class EmployeeBenefitRepository : Repository<EmployeeBenefit>, IEmployeeBenefitRepository
{
    public EmployeeBenefitRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeBenefit>> GetEmployeeBenefitsAsync(int employeeId)
    {
        return await _dbSet
            .Include(eb => eb.Benefit)
            .Where(eb => eb.EmployeeId == employeeId &&
                         (eb.EndDate == null || eb.EndDate > DateTime.Now))
            .ToListAsync();
    }

    public async Task<decimal> GetTotalBenefitsValueAsync(int employeeId)
    {
        return await _dbSet
            .Include(eb => eb.Benefit)
            .Where(eb => eb.EmployeeId == employeeId &&
                         (eb.EndDate == null || eb.EndDate > DateTime.Now))
            .SumAsync(eb => (decimal?)eb.Benefit.Value) ?? 0;
    }
}
