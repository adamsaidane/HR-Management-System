using System.ComponentModel.DataAnnotations;
using HRMS.Models;

namespace HRMS.ViewModels;

public class EquipmentAssignmentViewModel
{
    [Required]
    [Display(Name = "Équipement")]
    public int EquipmentId { get; set; }

    [Required]
    [Display(Name = "Employé")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "État")]
    public string Condition { get; set; }

    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string Notes { get; set; }

    public IEnumerable<Equipment>? AvailableEquipment { get; set; }
    public IEnumerable<Employee>? Employees { get; set; }
    public List<string> Conditions { get; set; } = new List<string> 
    { 
        "Neuf", "Bon", "Acceptable", "Endommagé" 
    };
}