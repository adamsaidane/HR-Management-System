using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class CandidateRepository : Repository<Candidate>, ICandidateRepository
{
    public CandidateRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<Candidate>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(c => c.JobOffer)
            .OrderByDescending(c => c.ApplicationDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidate>> GetByJobOfferAsync(int jobOfferId)
    {
        return await _dbSet
            .Where(c => c.JobOfferId == jobOfferId)
            .OrderByDescending(c => c.ApplicationDate)
            .ToListAsync();
    }

    public async Task<Candidate?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.JobOffer)
            .ThenInclude(j => j.Department)
            .Include(c => c.JobOffer)
            .ThenInclude(j => j.Position)
            .Include(c => c.Interviews)
            .FirstOrDefaultAsync(c => c.CandidateId == id);
    }
}