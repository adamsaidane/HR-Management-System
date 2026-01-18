using System.ComponentModel.DataAnnotations;
using HRMS.Enums;

namespace HRMS.Models;

public class Equipment
{
    [Key]
    public int EquipmentId { get; set; }

    [Required(ErrorMessage = "Le type d'équipement est requis")]
    [StringLength(50)]
    [Display(Name = "Type d'équipement")]
    public string EquipmentType { get; set; }

    [StringLength(50)]
    [Display(Name = "Numéro de série")]
    public string SerialNumber { get; set; }

    [StringLength(50)]
    [Display(Name = "Marque")]
    public string Brand { get; set; }

    [StringLength(50)]
    [Display(Name = "Modèle")]
    public string Model { get; set; }

    [Display(Name = "Date d'achat")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? PurchaseDate { get; set; }

    [Required]
    [Display(Name = "Statut")]
    public EquipmentStatus Status { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Date de modification")]
    public DateTime ModifiedDate { get; set; }

    // Navigation property
    public virtual ICollection<EquipmentAssignment> EquipmentAssignments { get; set; }

    public Equipment()
    {
        EquipmentAssignments = new HashSet<EquipmentAssignment>();
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
        Status = EquipmentStatus.Disponible;
    }
}