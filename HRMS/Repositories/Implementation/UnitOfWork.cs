using HRMS.Models;

namespace HRMS.Repositories.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly HRMSDbContext _context;

    public IEmployeeRepository Employees { get; }
    public IDepartmentRepository Departments { get; }
    public IPositionRepository Positions { get; }
    public ISalaryRepository Salaries { get; }
    public IBonusRepository Bonuses { get; }
    public IBenefitRepository Benefits { get; }
    public IEmployeeBenefitRepository EmployeeBenefits { get; }
    public IEquipmentRepository Equipments { get; }
    public IEquipmentAssignmentRepository EquipmentAssignments { get; }
    public IPromotionRepository Promotions { get; }
    public IJobOfferRepository JobOffers { get; }
    public ICandidateRepository Candidates { get; }
    public IInterviewRepository Interviews { get; }
    public IDocumentRepository Documents { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(HRMSDbContext context)
    {
        _context = context;
        
        Employees = new EmployeeRepository(_context);
        Departments = new DepartmentRepository(_context);
        Positions = new PositionRepository(_context);
        Salaries = new SalaryRepository(_context);
        Bonuses = new BonusRepository(_context);
        Benefits = new BenefitRepository(_context);
        EmployeeBenefits = new EmployeeBenefitRepository(_context);
        Equipments = new EquipmentRepository(_context);
        EquipmentAssignments = new EquipmentAssignmentRepository(_context);
        Promotions = new PromotionRepository(_context);
        JobOffers = new JobOfferRepository(_context);
        Candidates = new CandidateRepository(_context);
        Interviews = new InterviewRepository(_context);
        Documents = new DocumentRepository(_context);
        Users = new UserRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}