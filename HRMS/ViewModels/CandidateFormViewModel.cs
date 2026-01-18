using System.ComponentModel.DataAnnotations;
using HRMS.Models;

namespace HRMS.ViewModels;

public class CandidateFormViewModel
{
    public int CandidateId { get; set; }

    [Required]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Nom")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Téléphone")]
    public string Phone { get; set; }

    [Display(Name = "CV")]
    public string CVPath { get; set; }

    [Required]
    [Display(Name = "Offre d'emploi")]
    public int JobOfferId { get; set; }

    public IEnumerable<JobOffer> JobOffers { get; set; }
}