using HRMS.Models;

namespace HRMS.Repositories.Implementation;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(HRMSDbContext context) : base(context) { }
}