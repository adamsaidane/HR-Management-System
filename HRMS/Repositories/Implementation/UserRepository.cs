using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(HRMSDbContext context) : base(context) { }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Username == username);
    }
}
