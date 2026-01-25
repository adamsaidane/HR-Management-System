using HRMS.Enums;
using HRMS.Models;
using HRMS.ViewModels;

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
    Task<IEnumerable<Interview>> GetAllInterviewsAsync();
    Task UpdateInterviewResultAsync(int interviewId, InterviewResult result, string notes);
    Task<Employee> ConvertCandidateToEmployeeAsync(int candidateId, Employee employee);
    Task<JobOfferFormViewModel> GetJobOfferFormViewModelAsync();
    Task<CandidateFormViewModel> GetCandidateFormViewModelAsync();
    Task<CandidateDetailsViewModel> GetCandidateDetailsViewModelAsync(int candidateId);
    Task<Candidate> CreateCandidateWithCVAsync(CandidateFormViewModel model);
    Task<InterviewFormViewModel> GetInterviewFormViewModelAsync(int candidateId);
    Task<EmployeeFormViewModel> GetConvertToEmployeeViewModelAsync(int candidateId);
    Task<PaginatedList<Candidate>> GetCandidatesPaginatedAsync(
        int? jobOfferId,
        CandidateStatus? status,
        string searchString,
        int pageIndex = 1,
        int pageSize = 15);
        
    Task<PaginatedList<JobOffer>> GetJobOffersPaginatedAsync(
        JobOfferStatus? status,
        int? departmentId,
        ContractType? contractType,
        string searchString,
        int pageIndex = 1,
        int pageSize = 15);
    
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    
    Task AddCvFromCandidateToEmployeeAsync(int candidateId, Employee employee);
}
