using HRMS.Models;

namespace HRMS.ViewModels;

public class EmployeeDetailsViewModel
{
    public Employee Employee { get; set; }
    public decimal CurrentSalary { get; set; }
    public decimal TotalBenefits { get; set; }
    public decimal GrossSalary { get; set; }
    public List<Salary> SalaryHistory { get; set; }
    public List<Bonus> Bonuses { get; set; }
    public List<EmployeeBenefit> Benefits { get; set; }
    public List<EquipmentAssignment> Equipments { get; set; }
    public List<Promotion> Promotions { get; set; }
    public List<Document> Documents { get; set; }
}
