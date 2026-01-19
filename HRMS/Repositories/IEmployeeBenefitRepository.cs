using HRMS.Models;

namespace HRMS.Repositories;

public interface IEmployeeBenefitRepository : IRepository<EmployeeBenefit>
{
    Task<IEnumerable<EmployeeBenefit>> GetEmployeeBenefitsAsync(int employeeId);
    Task<decimal> GetTotalBenefitsValueAsync(int employeeId);
}
