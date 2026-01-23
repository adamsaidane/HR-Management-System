using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class RecruitmentService : IRecruitmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeService _employeeService;
    private readonly IWebHostEnvironment _environment;

    public RecruitmentService(IUnitOfWork unitOfWork, IEmployeeService employeeService, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _employeeService = employeeService;
        _environment = environment;
    }

    // JobOffers
    public async Task<IEnumerable<JobOffer>> GetAllJobOffersAsync()
    {
        return await _unitOfWork.JobOffers.GetAllWithDetailsAsync();
    }

    public async Task<IEnumerable<JobOffer>> GetActiveJobOffersAsync()
    {
        return await _unitOfWork.JobOffers.GetActiveJobOffersAsync();
    }
    
    public async Task<PaginatedList<JobOffer>> GetJobOffersPaginatedAsync(
        JobOfferStatus? status,
        string searchString,
        int pageIndex = 1,
        int pageSize = 6)
    {
        var jobOffers = await _unitOfWork.JobOffers.GetAllWithDetailsAsync();

        if (status.HasValue)
        {
            jobOffers = jobOffers.Where(j => j.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            jobOffers = jobOffers.Where(j =>
                j.Title.Contains(searchString) ||
                j.Description.Contains(searchString));
        }

        jobOffers = jobOffers.OrderByDescending(j => j.PostDate);

        return PaginatedList<JobOffer>.Create(jobOffers, pageIndex, pageSize);
    }

    public async Task<JobOffer?> GetJobOfferByIdAsync(int id)
    {
        return await _unitOfWork.JobOffers.GetByIdWithDetailsAsync(id);
    }

    public async Task CreateJobOfferAsync(JobOffer jobOffer)
    {
        jobOffer.PostDate = DateTime.Now;
        jobOffer.CreatedDate = DateTime.Now;
        jobOffer.ModifiedDate = DateTime.Now;
        await _unitOfWork.JobOffers.AddAsync(jobOffer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateJobOfferAsync(JobOffer jobOffer)
    {
        jobOffer.ModifiedDate = DateTime.Now;
        _unitOfWork.JobOffers.Update(jobOffer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CloseJobOfferAsync(int id)
    {
        var jobOffer = await _unitOfWork.JobOffers.GetByIdAsync(id);
        if (jobOffer != null)
        {
            jobOffer.Status = JobOfferStatus.Fermée;
            jobOffer.ModifiedDate = DateTime.Now;
            _unitOfWork.JobOffers.Update(jobOffer);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Candidates
    public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
    {
        return await _unitOfWork.Candidates.GetAllWithDetailsAsync();
    }

    public async Task<IEnumerable<Candidate>> GetCandidatesByJobOfferAsync(int jobOfferId)
    {
        return await _unitOfWork.Candidates.GetByJobOfferAsync(jobOfferId);
    }

    public async Task<Candidate?> GetCandidateByIdAsync(int id)
    {
        return await _unitOfWork.Candidates.GetByIdWithDetailsAsync(id);
    }

    public async Task CreateCandidateAsync(Candidate candidate)
    {
        candidate.ApplicationDate = DateTime.Now;
        candidate.Status = CandidateStatus.CandidatureReçue;
        await _unitOfWork.Candidates.AddAsync(candidate);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCandidateAsync(Candidate candidate)
    {
        _unitOfWork.Candidates.Update(candidate);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCandidateStatusAsync(int candidateId, CandidateStatus status)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate != null)
        {
            candidate.Status = status;
            _unitOfWork.Candidates.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Interviews
    public async Task ScheduleInterviewAsync(Interview interview)
    {
        interview.CreatedDate = DateTime.Now;
        interview.Result = InterviewResult.EnAttente;
        await _unitOfWork.Interviews.AddAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        // Mettre à jour le statut du candidat
        await UpdateCandidateStatusAsync(interview.CandidateId, CandidateStatus.Entretien);
    }

    public async Task<IEnumerable<Interview>> GetInterviewsByCandidateAsync(int candidateId)
    {
        return await _unitOfWork.Interviews.GetByCandidateAsync(candidateId);
    }

    public async Task UpdateInterviewResultAsync(int interviewId, InterviewResult result, string notes)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(interviewId);
        if (interview != null)
        {
            interview.Result = result;
            interview.Notes = notes;
            _unitOfWork.Interviews.Update(interview);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Conversion Candidat -> Employé
    public async Task<Employee> ConvertCandidateToEmployeeAsync(int candidateId, Employee employee)
    {
        var candidate = await GetCandidateByIdAsync(candidateId);
        if (candidate == null)
            throw new Exception("Candidat introuvable");

        employee.FirstName = candidate.FirstName;
        employee.LastName = candidate.LastName;
        employee.Gender =  candidate.Gender;
        employee.Email = candidate.Email;
        employee.Phone = candidate.Phone;

        await _employeeService.CreateEmployeeAsync(employee);

        await UpdateCandidateStatusAsync(candidateId, CandidateStatus.Accepté);

        return employee;
    }
    public async Task<JobOfferFormViewModel> GetJobOfferFormViewModelAsync()
    {
        return new JobOfferFormViewModel
        {
            Departments = await _employeeService.GetAllDepartmentsAsync(),
            Positions = await _employeeService.GetAllPositionsAsync()
        };
    }

    public async Task<CandidateFormViewModel> GetCandidateFormViewModelAsync()
    {
        return new CandidateFormViewModel
        {
            JobOffers = (await GetActiveJobOffersAsync()).ToList()
        };
    }

    public async Task<CandidateDetailsViewModel> GetCandidateDetailsViewModelAsync(int candidateId)
    {
        var candidate = await GetCandidateByIdAsync(candidateId);
        if (candidate == null)
            throw new Exception("Candidat introuvable");

        return new CandidateDetailsViewModel
        {
            Candidate = candidate,
            Interviews = (await GetInterviewsByCandidateAsync(candidateId)).ToList()
        };
    }

    public async Task<Candidate> CreateCandidateWithCVAsync(CandidateFormViewModel model)
    {
        var candidate = new Candidate
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            JobOfferId = model.JobOfferId.Value,
        };

        string? cvPath = null;
        if (model.CVFile != null && model.CVFile.Length > 0)
        {
            var folder = Path.Combine(_environment.WebRootPath, "Content/CVs");
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(model.CVFile.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await model.CVFile.CopyToAsync(stream);

            cvPath = "/Content/CVs/" + fileName;
        }

        candidate.CVPath = cvPath;
        
        await CreateCandidateAsync(candidate);
        return candidate;
    }

    public async Task<InterviewFormViewModel> GetInterviewFormViewModelAsync(int candidateId)
    {
        var candidate = await GetCandidateByIdAsync(candidateId);
        if (candidate == null)
            throw new Exception("Candidat introuvable");

        return new InterviewFormViewModel
        {
            Candidate = candidate,
            Interview = new Interview
            {
                CandidateId = candidateId,
                InterviewDate = DateTime.Now.AddDays(7)
            }
        };
    }

    public async Task<EmployeeFormViewModel> GetConvertToEmployeeViewModelAsync(int candidateId)
    {
        var candidate = await GetCandidateByIdAsync(candidateId);
        if (candidate == null)
            throw new Exception("Candidat introuvable");

        return new EmployeeFormViewModel
        {
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Gender = candidate.Gender,
            DateOfBirth = candidate.DateOfBirth,
            Email = candidate.Email,
            Phone = candidate.Phone,
            Address = candidate.Address,
            HireDate = DateTime.Today,
            Departments = await _employeeService.GetAllDepartmentsAsync(),
            Positions = await _employeeService.GetAllPositionsAsync()
        };
    }
    
    public async Task<PaginatedList<Candidate>> GetCandidatesPaginatedAsync(
        int? jobOfferId,
        CandidateStatus? status,
        string searchString,
        int pageIndex = 1,
        int pageSize = 15)
    {
        var candidates = await _unitOfWork.Candidates.GetAllWithDetailsAsync();

        if (jobOfferId.HasValue)
        {
            candidates = candidates.Where(c => c.JobOfferId == jobOfferId.Value);
        }

        if (status.HasValue)
        {
            candidates = candidates.Where(c => c.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            candidates = candidates.Where(c =>
                c.FirstName.Contains(searchString) ||
                c.LastName.Contains(searchString) ||
                c.Email.Contains(searchString));
        }

        candidates = candidates.OrderByDescending(c => c.ApplicationDate);

        return PaginatedList<Candidate>.Create(candidates, pageIndex, pageSize);
    }
}