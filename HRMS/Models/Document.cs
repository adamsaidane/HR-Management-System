using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Type de document")]
    public string DocumentType { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Nom du fichier")]
    public string FileName { get; set; }

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; }

    [Display(Name = "Date d'upload")]
    public DateTime UploadDate { get; set; }

    // Navigation property
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    public Document()
    {
        UploadDate = DateTime.Now;
    }
}