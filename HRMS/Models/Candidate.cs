using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Enums;

namespace HRMS.Models;

public class Candidate
{
    [Key]
    public int CandidateId { get; set; }

    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(50)]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(50)]
    [Display(Name = "Nom")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "L'email est requis")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "Adresse email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [StringLength(20)]
    [Display(Name = "Téléphone")]
    [Phone(ErrorMessage = "Numéro de téléphone invalide")]
    public string Phone { get; set; }

    [Display(Name = "CV")]
    public string CVPath { get; set; }

    [Required]
    [Display(Name = "Offre d'emploi")]
    public int JobOfferId { get; set; }

    [Required]
    [Display(Name = "Statut")]
    public CandidateStatus Status { get; set; }

    [Display(Name = "Date de candidature")]
    public DateTime ApplicationDate { get; set; }

    // Navigation properties
    [ForeignKey("JobOfferId")]
    public virtual JobOffer JobOffer { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public Candidate()
    {
        Interviews = new HashSet<Interview>();
        ApplicationDate = DateTime.Now;
        Status = CandidateStatus.CandidatureReçue;
    }
}