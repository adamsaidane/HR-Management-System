using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Enums;

namespace HRMS.Models;

public class JobOffer
{
    [Key]
    public int JobOfferId { get; set; }

    [Required(ErrorMessage = "Le titre est requis")]
    [StringLength(100)]
    [Display(Name = "Titre de l'offre")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Le type de contrat est requis")]
    [Display(Name = "Type de contrat")]
    public ContractType ContractType { get; set; }

    [Required(ErrorMessage = "La description est requise")]
    [Display(Name = "Description")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Département")]
    public int DepartmentId { get; set; }

    [Required]
    [Display(Name = "Poste")]
    public int PositionId { get; set; }

    [Display(Name = "Date de publication")]
    public DateTime PostDate { get; set; }

    [Display(Name = "Date d'expiration")]
    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }

    [Required]
    [Display(Name = "Statut")]
    public JobOfferStatus Status { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Date de modification")]
    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }

    [ForeignKey("PositionId")]
    public virtual Position? Position { get; set; }

    public virtual ICollection<Candidate> Candidates { get; set; }

    public JobOffer()
    {
        Candidates = new HashSet<Candidate>();
        PostDate = DateTime.Now;
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
        Status = JobOfferStatus.Ouverte;
    }
}