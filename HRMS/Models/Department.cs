using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class Department
{
    [Key]
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

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Date de modification")]
    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    [ForeignKey("ManagerId")]
    public virtual Employee Manager { get; set; }

    public virtual ICollection<Employee> Employees { get; set; }
    public virtual ICollection<JobOffer> JobOffers { get; set; }

    public Department()
    {
        Employees = new HashSet<Employee>();
        JobOffers = new HashSet<JobOffer>();
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
    }
}