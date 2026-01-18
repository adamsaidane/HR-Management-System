using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Service;

public interface IRecruitmentService
{
    // JobOffers
    IEnumerable<JobOffer> GetAllJobOffers();
    IEnumerable<JobOffer> GetActiveJobOffers();
    JobOffer GetJobOfferById(int id);
    void CreateJobOffer(JobOffer jobOffer);
    void UpdateJobOffer(JobOffer jobOffer);
    void CloseJobOffer(int id);

    // Candidates
    IEnumerable<Candidate> GetAllCandidates();
    IEnumerable<Candidate> GetCandidatesByJobOffer(int jobOfferId);
    Candidate GetCandidateById(int id);
    void CreateCandidate(Candidate candidate);
    void UpdateCandidate(Candidate candidate);
    void UpdateCandidateStatus(int candidateId, CandidateStatus status);

    // Interviews
    void ScheduleInterview(Interview interview);
    IEnumerable<Interview> GetInterviewsByCandidate(int candidateId);
    void UpdateInterviewResult(int interviewId, InterviewResult result, string notes);

    // Conversion
    Employee ConvertCandidateToEmployee(int candidateId, Employee employee);
}