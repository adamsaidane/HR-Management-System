using HRMS.Models;
using HRMS.Repositories;

namespace HRMS.Repositories.Implementation;

public class PositionRepository : Repository<Position>, IPositionRepository
{
    public PositionRepository(HRMSDbContext context) : base(context) { }
}
