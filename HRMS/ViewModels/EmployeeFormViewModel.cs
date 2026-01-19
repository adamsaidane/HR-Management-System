using System.ComponentModel.DataAnnotations;
using HRMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HRMS.ViewModels;

public class EmployeeFormViewModel
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Le prénom est requis")]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom est requis")]
    [Display(Name = "Nom")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "La date de naissance est requise")]
    [Display(Name = "Date de naissance")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Adresse")]
    public string Address { get; set; }

    [Display(Name = "Téléphone")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le département est requis")]
    [Display(Name = "Département")]
    public int? DepartmentId { get; set; }

    [Required(ErrorMessage = "Le poste est requis")]
    [Display(Name = "Poste")]
    public int? PositionId { get; set; }

    [Required(ErrorMessage = "La date d'embauche est requise")]
    [Display(Name = "Date d'embauche")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    [Required(ErrorMessage = "Le type de contrat est requis")]
    [Display(Name = "Type de contrat")]
    public string ContractType { get; set; }

    [Display(Name = "Photo")]
    [ValidateNever]
    public IFormFile Photo { get; set; }

    [ValidateNever]
    public IEnumerable<Department> Departments { get; set; }
    [ValidateNever]
    public IEnumerable<Position> Positions { get; set; }
    [ValidateNever]
    public List<string> ContractTypes { get; set; } = new List<string> 
    { 
        "CDI", "CDD", "Stage", "Freelance", "Intérim" 
    };
}