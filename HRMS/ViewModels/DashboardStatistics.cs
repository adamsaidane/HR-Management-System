namespace HRMS.ViewModels;

public class DashboardStatistics
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int SuspendedEmployees { get; set; }
    public int ResignedEmployees { get; set; }
    public int OpenJobOffers { get; set; }
    public int PendingCandidates { get; set; }
    public int ScheduledInterviews { get; set; }
    public decimal TotalMonthlySalary { get; set; }
    public decimal AverageSalary { get; set; }
    public int TotalEquipment { get; set; }
    public int AssignedEquipment { get; set; }
    public int AvailableEquipment { get; set; }
    public Dictionary<string, int> EmployeesByDepartment { get; set; }
    public Dictionary<string, int> EmployeesByStatus { get; set; }
    public Dictionary<string, int> EquipmentByType { get; set; }
    public List<SalaryEvolution> SalaryEvolutionData { get; set; }
    public List<RecentPromotion> RecentPromotions { get; set; }
}