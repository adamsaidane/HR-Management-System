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
}