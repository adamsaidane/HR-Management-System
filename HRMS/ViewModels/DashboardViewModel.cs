namespace HRMS.ViewModels;

public class DashboardViewModel
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int OpenJobOffers { get; set; }
    public int PendingCandidates { get; set; }
    public decimal TotalMonthlySalary { get; set; }
    public decimal AverageSalary { get; set; }
    public int AssignedEquipment { get; set; }
    public int AvailableEquipment { get; set; }
    public int TotalEquipment { get; set; }
    public Dictionary<string, int> EmployeesByDepartment { get; set; }
    public Dictionary<string, int> EmployeesByStatus { get; set; }
    public List<SalaryEvolution> SalaryEvolution { get; set; }
    public List<RecentPromotion> RecentPromotions { get; set; }
    
    public Dictionary<string, int> EmployeesByGender { get; set; }
    public Dictionary<string, int> EmployeesByAgeRange { get; set; }
    public Dictionary<string, int> EmployeesByContractType { get; set; }
    public Dictionary<string, int> EmployeesBySeniority { get; set; }

    public Dictionary<string, int> CandidatesByStage { get; set; }
    public Dictionary<string, int> JobOffersByDepartment { get; set; }

    public Dictionary<string, decimal> SalaryByDepartment { get; set; }

    public Dictionary<string, int> EquipmentByStatus { get; set; }
    
    public Dictionary<string, int> HiringTrendByMonth { get; set; }
    public Dictionary<string, decimal> BonusesByDepartment { get; set; }
    public Dictionary<string, int> BonusesByType { get; set; }
    public Dictionary<string, int> DocumentsByType { get; set; }
    public Dictionary<string, decimal> SalaryDistribution { get; set; }
    public Dictionary<string, int> PromotionsByDepartment { get; set; }
    public Dictionary<string, int> InterviewSuccessRate { get; set; }
    public Dictionary<string, int> EquipmentByType { get; set; }
    public Dictionary<string, int> BenefitUtilization { get; set; }
    public Dictionary<int, int> EmployeeGrowthByYear { get; set; }
    public Dictionary<string, int> CandidatesApplicationTrend { get; set; }
    public Dictionary<string, decimal> AverageSalaryByDepartment { get; set; }
    public Dictionary<string, decimal> BonusTrendByMonth { get; set; }
    public Dictionary<string, int> EquipmentAssignmentsByDepartment { get; set; }
    public Dictionary<string, int> ActiveJobOffersByContractType { get; set; }
    public Dictionary<string, int> InterviewsByMonth { get; set; }
    public Dictionary<string, int> PromotionTrendByMonth { get; set; }
    public Dictionary<string, decimal> SalaryIncreaseTrend { get; set; }
    public Dictionary<string, int> EmployeesByAgeAndGender { get; set; }
    public Dictionary<string, decimal> BenefitCostByDepartment { get; set; }
    public Dictionary<string, int> EquipmentPurchaseByYear { get; set; }
    public Dictionary<string, decimal> AverageBonusByDepartment { get; set; }
    public Dictionary<string, int> EmployeeRetentionByYear { get; set; }
    public Dictionary<string, int> InterviewsScheduledVsCompleted { get; set; }
    public Dictionary<string, decimal> SalaryGrowthByDepartment { get; set; }
    public Dictionary<string, int> BenefitsByType { get; set; }
}