using HRMS.Models;

namespace HRMS.Repositories;

public interface ISalaryRepository : IRepository<Salary>
{
    Task<Salary?> GetCurrentSalaryAsync(int employeeId);
    Task<IEnumerable<Salary>> GetSalaryHistoryAsync(int employeeId);
}