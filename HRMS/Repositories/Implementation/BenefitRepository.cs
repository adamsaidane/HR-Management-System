using HRMS.Models;
using HRMS.Repositories;

namespace HRMS.Repositories.Implementation;

public class BenefitRepository : Repository<Benefit>, IBenefitRepository
{
    public BenefitRepository(HRMSDbContext context) : base(context) { }
}
