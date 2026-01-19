using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Service;

public interface IRecruitmentService
{
    Task<IEnumerable<JobOffer>> GetAllJobOffersAsync();
    Task<IEnumerable<JobOffer>> GetActiveJobOffersAsync();
    Task<JobOffer?> GetJobOfferByIdAsync(int id);
    Task CreateJobOfferAsync(JobOffer jobOffer);
    Task UpdateJobOfferAsync(JobOffer jobOffer);
    Task CloseJobOfferAsync(int id);
    Task<IEnumerable<Candidate>> GetAllCandidatesAsync();
    Task<IEnumerable<Candidate>> GetCandidatesByJobOfferAsync(int jobOfferId);
    Task<Candidate?> GetCandidateByIdAsync(int id);
    Task CreateCandidateAsync(Candidate candidate);
    Task UpdateCandidateAsync(Candidate candidate);
    Task UpdateCandidateStatusAsync(int candidateId, CandidateStatus status);
    Task ScheduleInterviewAsync(Interview interview);
    Task<IEnumerable<Interview>> GetInterviewsByCandidateAsync(int candidateId);
    Task UpdateInterviewResultAsync(int interviewId, InterviewResult result, string notes);
    Task<Employee> ConvertCandidateToEmployeeAsync(int candidateId, Employee employee);
}