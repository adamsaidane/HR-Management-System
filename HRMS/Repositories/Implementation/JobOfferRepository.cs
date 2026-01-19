using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class JobOfferRepository : Repository<JobOffer>, IJobOfferRepository
{
    public JobOfferRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<JobOffer>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(j => j.Department)
            .Include(j => j.Position)
            .Include(j => j.Candidates)
            .OrderByDescending(j => j.PostDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobOffer>> GetActiveJobOffersAsync()
    {
        return await _dbSet
            .Include(j => j.Department)
            .Include(j => j.Position)
            .Where(j => j.Status == JobOfferStatus.Ouverte)
            .OrderByDescending(j => j.PostDate)
            .ToListAsync();
    }

    public async Task<JobOffer?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(j => j.Department)
            .Include(j => j.Position)
            .Include(j => j.Candidates)
            .FirstOrDefaultAsync(j => j.JobOfferId == id);
    }
}
