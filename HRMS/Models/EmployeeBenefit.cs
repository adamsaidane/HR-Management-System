using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class EmployeeBenefit
{
    [Key]
    public int EmployeeBenefitId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int BenefitId { get; set; }

    [Required(ErrorMessage = "La date de début est requise")]
    [Display(Name = "Date de début")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    [Display(Name = "Date de fin")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? EndDate { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    [ForeignKey("BenefitId")]
    public virtual Benefit Benefit { get; set; }

    public EmployeeBenefit()
    {
        CreatedDate = DateTime.Now;
        StartDate = DateTime.Now;
    }
}