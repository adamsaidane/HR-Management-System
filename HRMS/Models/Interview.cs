using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Enums;

namespace HRMS.Models;

public class Interview
{
    [Key]
    public int InterviewId { get; set; }

    [Required]
    public int CandidateId { get; set; }

    [Required(ErrorMessage = "La date de l'entretien est requise")]
    [Display(Name = "Date de l'entretien")]
    public DateTime InterviewDate { get; set; }

    [StringLength(200)]
    [Display(Name = "Lieu")]
    public string Location { get; set; }

    [StringLength(100)]
    [Display(Name = "Nom de l'interviewer")]
    public string InterviewerName { get; set; }

    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string Notes { get; set; }

    [Display(Name = "Résultat")]
    public InterviewResult? Result { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedDate { get; set; }

    // Navigation property
    [ForeignKey("CandidateId")]
    public virtual Candidate Candidate { get; set; }

    public Interview()
    {
        CreatedDate = DateTime.Now;
        Result = InterviewResult.EnAttente;
    }
}