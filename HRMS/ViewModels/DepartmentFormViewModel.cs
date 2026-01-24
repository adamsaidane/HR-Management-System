using System.ComponentModel.DataAnnotations;
using HRMS.Models;

namespace HRMS.ViewModels;

public class DepartmentFormViewModel
{
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Le nom du département est requis")]
    [StringLength(100)]
    [Display(Name = "Nom du département")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Le code est requis")]
    [StringLength(10)]
    [Display(Name = "Code")]
    public string Code { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Manager")]
    public int? ManagerId { get; set; }

    public List<Employee> AvailableManagers { get; set; } = new();
}

public class PositionFormViewModel
{
    public int PositionId { get; set; }

    [Required(ErrorMessage = "Le titre du poste est requis")]
    [StringLength(100)]
    [Display(Name = "Titre du poste")]
    public string Title { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Le salaire de base est requis")]
    [Display(Name = "Salaire de base")]
    [Range(0, double.MaxValue, ErrorMessage = "Le salaire doit être positif")]
    public decimal BaseSalary { get; set; }

    [Required(ErrorMessage = "Le département est requis")]
    [Display(Name = "Département")]
    public int DepartmentId { get; set; }

    public List<Department> Departments { get; set; } = new();
}
