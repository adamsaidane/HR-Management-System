using HRMS.Models;

namespace HRMS.Repositories;

public interface IJobOfferRepository : IRepository<JobOffer>
{
    Task<IEnumerable<JobOffer>> GetAllWithDetailsAsync();
    Task<IEnumerable<JobOffer>> GetActiveJobOffersAsync();
    Task<JobOffer?> GetByIdWithDetailsAsync(int id);
}
