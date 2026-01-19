using HRMS.Models;

namespace HRMS.Repositories;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<IEnumerable<Candidate>> GetAllWithDetailsAsync();
    Task<IEnumerable<Candidate>> GetByJobOfferAsync(int jobOfferId);
    Task<Candidate?> GetByIdWithDetailsAsync(int id);
}