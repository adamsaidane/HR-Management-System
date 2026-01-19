using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class InterviewRepository : Repository<Interview>, IInterviewRepository
{
    public InterviewRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<Interview>> GetByCandidateAsync(int candidateId)
    {
        return await _dbSet
            .Where(i => i.CandidateId == candidateId)
            .OrderByDescending(i => i.InterviewDate)
            .ToListAsync();
    }
}
