namespace HRMS.Repositories;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    IPositionRepository Positions { get; }
    ISalaryRepository Salaries { get; }
    IBonusRepository Bonuses { get; }
    IBenefitRepository Benefits { get; }
    IEmployeeBenefitRepository EmployeeBenefits { get; }
    IEquipmentRepository Equipments { get; }
    IEquipmentAssignmentRepository EquipmentAssignments { get; }
    IPromotionRepository Promotions { get; }
    IJobOfferRepository JobOffers { get; }
    ICandidateRepository Candidates { get; }
    IInterviewRepository Interviews { get; }
    IDocumentRepository Documents { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
    int SaveChanges();
}