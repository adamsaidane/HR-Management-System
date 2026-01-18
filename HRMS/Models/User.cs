using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Enums;

namespace HRMS.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
    [StringLength(50)]
    [Display(Name = "Nom d'utilisateur")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [StringLength(500)]
    [DataType(DataType.Password)]
    public string PasswordHash { get; set; }

    [Required(ErrorMessage = "L'email est requis")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "Adresse email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Employé associé")]
    public int? EmployeeId { get; set; }

    [Required]
    [Display(Name = "Rôle")]
    public UserRole Role { get; set; }

    [Display(Name = "Actif")]
    public bool IsActive { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Date de modification")]
    public DateTime ModifiedDate { get; set; }

    // Navigation property
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    public User()
    {
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
        IsActive = true;
        Role = UserRole.Employé;
    }
}