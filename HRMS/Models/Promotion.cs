using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class Promotion
{
    [Key]
    public int PromotionId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Ancien poste")]
    public int OldPositionId { get; set; }

    [Required]
    [Display(Name = "Nouveau poste")]
    public int NewPositionId { get; set; }

    [Required]
    [Display(Name = "Ancien salaire")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal OldSalary { get; set; }

    [Required]
    [Display(Name = "Nouveau salaire")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal NewSalary { get; set; }

    [Required(ErrorMessage = "La date de promotion est requise")]
    [Display(Name = "Date de promotion")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime PromotionDate { get; set; }

    [StringLength(500)]
    [Display(Name = "Justification")]
    [DataType(DataType.MultilineText)]
    public string Justification { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    [ForeignKey("OldPositionId")]
    public virtual Position OldPosition { get; set; }

    [ForeignKey("NewPositionId")]
    public virtual Position NewPosition { get; set; }

    public Promotion()
    {
        CreatedDate = DateTime.Now;
        PromotionDate = DateTime.Now;
    }
}