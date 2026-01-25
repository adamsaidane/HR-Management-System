using HRMS.Models;

namespace HRMS.Repositories;

public interface IInterviewRepository : IRepository<Interview>
{
    Task<IEnumerable<Interview>> GetByCandidateAsync(int candidateId);
    Task<IEnumerable<Interview>> GetAllWithDetailsAsync();
}