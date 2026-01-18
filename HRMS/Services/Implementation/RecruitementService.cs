using HRMS.Enums;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class RecruitmentService : IRecruitmentService
    {
        private readonly HRMSDbContext _context;
        private readonly IEmployeeService _employeeService;

        public RecruitmentService(HRMSDbContext context, IEmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        // JobOffers
        public IEnumerable<JobOffer> GetAllJobOffers()
        {
            return _context.JobOffers
                .Include(j => j.Department)
                .Include(j => j.Position)
                .OrderByDescending(j => j.PostDate)
                .ToList();
        }

        public IEnumerable<JobOffer> GetActiveJobOffers()
        {
            return _context.JobOffers
                .Include(j => j.Department)
                .Include(j => j.Position)
                .Where(j => j.Status == JobOfferStatus.Ouverte)
                .OrderByDescending(j => j.PostDate)
                .ToList();
        }

        public JobOffer GetJobOfferById(int id)
        {
            return _context.JobOffers
                .Include(j => j.Department)
                .Include(j => j.Position)
                .Include(j => j.Candidates)
                .FirstOrDefault(j => j.JobOfferId == id);
        }

        public void CreateJobOffer(JobOffer jobOffer)
        {
            jobOffer.PostDate = DateTime.Now;
            jobOffer.CreatedDate = DateTime.Now;
            jobOffer.ModifiedDate = DateTime.Now;
            _context.JobOffers.Add(jobOffer);
            _context.SaveChanges();
        }

        public void UpdateJobOffer(JobOffer jobOffer)
        {
            jobOffer.ModifiedDate = DateTime.Now;
            _context.Entry(jobOffer).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void CloseJobOffer(int id)
        {
            var jobOffer = _context.JobOffers.Find(id);
            if (jobOffer != null)
            {
                jobOffer.Status = JobOfferStatus.Fermée;
                jobOffer.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        // Candidates
        public IEnumerable<Candidate> GetAllCandidates()
        {
            return _context.Candidates
                .Include(c => c.JobOffer)
                .OrderByDescending(c => c.ApplicationDate)
                .ToList();
        }

        public IEnumerable<Candidate> GetCandidatesByJobOffer(int jobOfferId)
        {
            return _context.Candidates
                .Where(c => c.JobOfferId == jobOfferId)
                .OrderByDescending(c => c.ApplicationDate)
                .ToList();
        }

        public Candidate GetCandidateById(int id)
        {
            return _context.Candidates
                .Include(c => c.JobOffer)
                .Include(c => c.Interviews)
                .FirstOrDefault(c => c.CandidateId == id);
        }

        public void CreateCandidate(Candidate candidate)
        {
            candidate.ApplicationDate = DateTime.Now;
            candidate.Status = CandidateStatus.CandidatureReçue;
            _context.Candidates.Add(candidate);
            _context.SaveChanges();
        }

        public void UpdateCandidate(Candidate candidate)
        {
            _context.Entry(candidate).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void UpdateCandidateStatus(int candidateId, CandidateStatus status)
        {
            var candidate = _context.Candidates.Find(candidateId);
            if (candidate != null)
            {
                candidate.Status = status;
                _context.SaveChanges();
            }
        }

        // Interviews
        public void ScheduleInterview(Interview interview)
        {
            interview.CreatedDate = DateTime.Now;
            interview.Result = InterviewResult.EnAttente;
            _context.Interviews.Add(interview);
            _context.SaveChanges();

            // Mettre à jour le statut du candidat
            UpdateCandidateStatus(interview.CandidateId, CandidateStatus.Entretien);
        }

        public IEnumerable<Interview> GetInterviewsByCandidate(int candidateId)
        {
            return _context.Interviews
                .Where(i => i.CandidateId == candidateId)
                .OrderByDescending(i => i.InterviewDate)
                .ToList();
        }

        public void UpdateInterviewResult(int interviewId, InterviewResult result, string notes)
        {
            var interview = _context.Interviews.Find(interviewId);
            if (interview != null)
            {
                interview.Result = result;
                interview.Notes = notes;
                _context.SaveChanges();
            }
        }

        // Conversion Candidat -> Employé
        public Employee ConvertCandidateToEmployee(int candidateId, Employee employee)
        {
            var candidate = GetCandidateById(candidateId);
            if (candidate == null)
                throw new Exception("Candidat introuvable");

            // Créer l'employé
            employee.FirstName = candidate.FirstName;
            employee.LastName = candidate.LastName;
            employee.Email = candidate.Email;
            employee.Phone = candidate.Phone;
            
            _employeeService.CreateEmployee(employee);

            // Mettre à jour le statut du candidat
            UpdateCandidateStatus(candidateId, CandidateStatus.Accepté);

            return employee;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }