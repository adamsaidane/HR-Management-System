using HRMS.ViewModels;

namespace HRMS.Service;

public interface IDashboardService
{
    Task<DashboardStatistics> GetDashboardStatisticsAsync();
}