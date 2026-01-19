using HRMS.Models;

namespace HRMS.Repositories;

public interface IPromotionRepository : IRepository<Promotion>
{
    Task<IEnumerable<Promotion>> GetAllWithDetailsAsync();
    Task<IEnumerable<Promotion>> GetEmployeePromotionsAsync(int employeeId);
    Task<IEnumerable<Promotion>> GetRecentPromotionsAsync(int count);
}
