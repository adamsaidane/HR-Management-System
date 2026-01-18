using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class Salary
{
    [Key]
    public int SalaryId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Le salaire de base est requis")]
    [Display(Name = "Salaire de base")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, 999999999.99, ErrorMessage = "Le salaire doit être positif")]
    public decimal BaseSalary { get; set; }

    [Required(ErrorMessage = "La date d'effet est requise")]
    [Display(Name = "Date d'effet")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EffectiveDate { get; set; }

    [Display(Name = "Date de fin")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? EndDate { get; set; }

    [StringLength(500)]
    [Display(Name = "Justification")]
    [DataType(DataType.MultilineText)]
    public string Justification { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    // Navigation property
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    public Salary()
    {
        CreatedDate = DateTime.Now;
        EffectiveDate = DateTime.Now;
    }
}