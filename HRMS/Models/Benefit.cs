using System.ComponentModel.DataAnnotations;

namespace HRMS.Models;

public class Benefit
{
    [Key]
    public int BenefitId { get; set; }

    [Required(ErrorMessage = "Le type d'avantage est requis")]
    [StringLength(50)]
    [Display(Name = "Type d'avantage")]
    public string BenefitType { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }

    [Required(ErrorMessage = "La valeur est requise")]
    [Display(Name = "Valeur")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, 999999999.99, ErrorMessage = "La valeur doit être positive")]
    public decimal Value { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Date de modification")]
    public DateTime ModifiedDate { get; set; }

    // Navigation property
    public virtual ICollection<EmployeeBenefit> EmployeeBenefits { get; set; }

    public Benefit()
    {
        EmployeeBenefits = new HashSet<EmployeeBenefit>();
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
    }
}