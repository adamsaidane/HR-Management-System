using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class Bonus
{
    [Key]
    public int BonusId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Le type de prime est requis")]
    [StringLength(50)]
    [Display(Name = "Type de prime")]
    public string BonusType { get; set; }

    [Required(ErrorMessage = "Le montant est requis")]
    [Display(Name = "Montant")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, 999999999.99, ErrorMessage = "Le montant doit être positif")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "La date est requise")]
    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    // Navigation property
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    public Bonus()
    {
        CreatedDate = DateTime.Now;
        Date = DateTime.Now;
    }
}