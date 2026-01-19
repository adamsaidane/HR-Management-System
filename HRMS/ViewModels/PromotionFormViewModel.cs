using System.ComponentModel.DataAnnotations;
using HRMS.Models;

namespace HRMS.ViewModels;

public class PromotionFormViewModel
{
    [Required]
    [Display(Name = "Employé")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Nouveau poste")]
    public int NewPositionId { get; set; }

    [Required]
    [Display(Name = "Nouveau salaire")]
    [Range(0, 999999999.99)]
    public decimal NewSalary { get; set; }

    [Required]
    [Display(Name = "Date de promotion")]
    [DataType(DataType.Date)]
    public DateTime PromotionDate { get; set; }

    [Display(Name = "Justification")]
    [DataType(DataType.MultilineText)]
    public string Justification { get; set; }

    // Données actuelles
    public string? CurrentPosition { get; set; }
    public decimal CurrentSalary { get; set; }

    // Listes
    public IEnumerable<Employee>? Employees { get; set; }
    public IEnumerable<Position>? Positions { get; set; }
}