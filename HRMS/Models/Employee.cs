using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Enums;

namespace HRMS.Models;

public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Le matricule est requis")]
        [StringLength(20)]
        [Display(Name = "Matricule")]
        public string Matricule { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50)]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50)]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "La date de naissance est requise")]
        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(200)]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [StringLength(20)]
        [Display(Name = "Téléphone")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Département")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Poste")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "La date d'embauche est requise")]
        [Display(Name = "Date d'embauche")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Le type de contrat est requis")]
        [StringLength(50)]
        [Display(Name = "Type de contrat")]
        public string ContractType { get; set; }

        [Required]
        [Display(Name = "Statut")]
        public EmployeeStatus Status { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Photo")]
        public string PhotoPath { get; set; }

        [Display(Name = "Date de création")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Date de modification")]
        public DateTime ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
        public virtual ICollection<Bonus> Bonuses { get; set; }
        public virtual ICollection<EmployeeBenefit> EmployeeBenefits { get; set; }
        public virtual ICollection<EquipmentAssignment> EquipmentAssignments { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        [Display(Name = "Nom complet")]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        [Display(Name = "Âge")]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public Employee()
        {
            Documents = new HashSet<Document>();
            Salaries = new HashSet<Salary>();
            Bonuses = new HashSet<Bonus>();
            EmployeeBenefits = new HashSet<EmployeeBenefit>();
            EquipmentAssignments = new HashSet<EquipmentAssignment>();
            Promotions = new HashSet<Promotion>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            Status = EmployeeStatus.Actif;
        }
    }