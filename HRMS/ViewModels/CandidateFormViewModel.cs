using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HRMS.ViewModels;

public class CandidateFormViewModel
{
    public int CandidateId { get; set; }

    [Display(Name = "Prénom")]
    public string FirstName { get; set; }

    [Display(Name = "Nom")]
    public string LastName { get; set; }

    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Téléphone")]
    public string Phone { get; set; }

    [StringLength(200)]
    [Display(Name = "Adresse")]
    public string Address { get; set; }
    
    [Display(Name = "CV")]
    public IFormFile CVFile { get; set; }

    [Display(Name = "Offre d'emploi")]
    public int? JobOfferId { get; set; }
    [NotMapped]
    [ValidateNever]
    public IEnumerable<JobOffer> JobOffers { get; set; }
}