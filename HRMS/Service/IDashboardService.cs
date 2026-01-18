using HRMS.ViewModels;

namespace HRMS.Service;

public interface IDashboardService
{
    DashboardStatistics GetDashboardStatistics();
    List<SalaryEvolution> GetSalaryEvolutionLastYear();
    List<RecentPromotion> GetRecentPromotions(int count = 10);
    Dictionary<string, int> GetEmployeesByDepartment();
    Dictionary<string, decimal> GetDepartmentSalaryDistribution();
}