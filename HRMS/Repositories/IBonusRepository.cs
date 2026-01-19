using HRMS.Models;

namespace HRMS.Repositories;

public interface IBonusRepository : IRepository<Bonus>
{
    Task<IEnumerable<Bonus>> GetByEmployeeAsync(int employeeId);
    Task<decimal> GetTotalBonusesAsync(int employeeId, int year);
}
