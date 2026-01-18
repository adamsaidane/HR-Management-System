using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class EquipmentAssignment
{
    [Key]
    public int AssignmentId { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "La date d'affectation est requise")]
    [Display(Name = "Date d'affectation")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime AssignmentDate { get; set; }

    [Display(Name = "Date de retour")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? ReturnDate { get; set; }

    [Required(ErrorMessage = "L'état est requis")]
    [StringLength(50)]
    [Display(Name = "État")]
    public string Condition { get; set; }

    [StringLength(500)]
    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string Notes { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    [ForeignKey("EquipmentId")]
    public virtual Equipment Equipment { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    public EquipmentAssignment()
    {
        CreatedDate = DateTime.Now;
        AssignmentDate = DateTime.Now;
    }
}