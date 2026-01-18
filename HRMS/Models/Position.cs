using System.ComponentModel.DataAnnotations;

namespace HRMS.Models;

public class Position
{
    [Key]
    public int PositionId { get; set; }

    [Required(ErrorMessage = "Le titre du poste est requis")]
    [StringLength(100)]
    [Display(Name = "Titre du poste")]
    public string Title { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Salaire de base")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal BaseSalary { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Date de modification")]
    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<Employee> Employees { get; set; }
    public virtual ICollection<JobOffer> JobOffers { get; set; }

    public Position()
    {
        Employees = new HashSet<Employee>();
        JobOffers = new HashSet<JobOffer>();
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
    }
}