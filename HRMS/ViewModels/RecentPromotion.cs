namespace HRMS.ViewModels;

public class RecentPromotion
{
    public string EmployeeName { get; set; }
    public string OldPosition { get; set; }
    public string NewPosition { get; set; }
    public DateTime PromotionDate { get; set; }
    public decimal SalaryIncrease { get; set; }
}