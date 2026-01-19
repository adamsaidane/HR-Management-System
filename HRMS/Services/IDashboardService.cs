using HRMS.ViewModels;

namespace HRMS.Service;

public interface IDashboardService
{
    Task<DashboardStatistics> GetDashboardStatisticsAsync();
    Task<List<SalaryEvolution>> GetSalaryEvolutionLastYearAsync();
    Task<List<RecentPromotion>> GetRecentPromotionsAsync(int count = 10);
    Task<Dictionary<string, int>> GetEmployeesByDepartmentAsync();
    Task<Dictionary<string, decimal>> GetDepartmentSalaryDistributionAsync();
}