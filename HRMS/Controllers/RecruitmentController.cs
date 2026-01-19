using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using HRMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class RecruitmentController : Controller
{
    private readonly IRecruitmentService _recruitmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public RecruitmentController(
        IRecruitmentService recruitmentService,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment environment)
    {
        _recruitmentService = recruitmentService;
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    // ==================== Offres d'emploi ====================

    public async Task<IActionResult> JobOffers()
    {
        var jobOffers = await _recruitmentService.GetAllJobOffersAsync();
        return View(jobOffers);
    }

    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CreateJobOffer()
    {
        ViewBag.Departments = await _unitOfWork.Departments.GetAllAsync();
        ViewBag.Positions = await _unitOfWork.Positions.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CreateJobOffer(JobOffer model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Positions = await _unitOfWork.Positions.GetAllAsync();
            return View(model);
        }

        await _recruitmentService.CreateJobOfferAsync(model);
        TempData["Success"] = "Offre d'emploi créée avec succès!";
        return RedirectToAction(nameof(JobOffers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CloseJobOffer(int id)
    {
        await _recruitmentService.CloseJobOfferAsync(id);
        TempData["Success"] = "Offre fermée avec succès!";
        return RedirectToAction(nameof(JobOffers));
    }

    // ==================== Candidats ====================

    public async Task<IActionResult> Candidates(int? jobOfferId)
    {
        var candidates = jobOfferId.HasValue
            ? await _recruitmentService.GetCandidatesByJobOfferAsync(jobOfferId.Value)
            : await _recruitmentService.GetAllCandidatesAsync();

        ViewBag.JobOffers = await _unitOfWork.JobOffers.GetAllAsync();
        ViewBag.SelectedJobOffer = jobOfferId;
        return View(candidates);
    }

    public async Task<IActionResult> CandidateDetails(int id)
    {
        var candidate = await _recruitmentService.GetCandidateByIdAsync(id);
        if (candidate == null)
            return NotFound();

        ViewBag.Interviews = await _recruitmentService.GetInterviewsByCandidateAsync(id);
        return View(candidate);
    }

    public async Task<IActionResult> CreateCandidate()
    {
        return View(new CandidateFormViewModel
        {
            JobOffers = (await _recruitmentService.GetActiveJobOffersAsync()).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCandidate(CandidateFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.JobOffers = (await _recruitmentService.GetActiveJobOffersAsync()).ToList();
            return View(model);
        }

        var candidate = new Candidate
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            JobOfferId = model.JobOfferId.Value
        };

        if (model.CVFile != null && model.CVFile.Length > 0)
        {
            var folder = Path.Combine(_environment.WebRootPath, "Content/CVs");
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(model.CVFile.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await model.CVFile.CopyToAsync(stream);

            candidate.CVPath = "/Content/CVs/" + fileName;
        }

        await _recruitmentService.CreateCandidateAsync(candidate);
        TempData["Success"] = "Candidature enregistrée avec succès!";
        return RedirectToAction(nameof(Candidates));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> UpdateCandidateStatus(int candidateId, CandidateStatus status)
    {
        await _recruitmentService.UpdateCandidateStatusAsync(candidateId, status);
        TempData["Success"] = "Statut mis à jour!";
        return RedirectToAction(nameof(CandidateDetails), new { id = candidateId });
    }

    // ==================== Entretiens ====================

    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> ScheduleInterview(int candidateId)
    {
        var candidate = await _recruitmentService.GetCandidateByIdAsync(candidateId);
        if (candidate == null)
            return NotFound();

        ViewBag.Candidate = candidate;

        return View(new Interview
        {
            CandidateId = candidateId,
            InterviewDate = DateTime.Now.AddDays(7)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> ScheduleInterview(Interview interview)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Candidate = await _recruitmentService.GetCandidateByIdAsync(interview.CandidateId);
            return View(interview);
        }

        await _recruitmentService.ScheduleInterviewAsync(interview);
        TempData["Success"] = "Entretien programmé avec succès!";
        return RedirectToAction(nameof(CandidateDetails), new { id = interview.CandidateId });
    }

    // ==================== Conversion Candidat -> Employé ====================

    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> ConvertToEmployee(int candidateId)
    {
        var candidate = await _recruitmentService.GetCandidateByIdAsync(candidateId);
        if (candidate == null)
            return NotFound();

        ViewBag.Candidate = candidate;

        return View(new EmployeeFormViewModel
        {
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            Address = candidate.Address,
            HireDate = DateTime.Today,
            Departments = (await _unitOfWork.Departments.GetAllAsync()).ToList(),
            Positions = (await _unitOfWork.Positions.GetAllAsync()).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> ConvertToEmployee(int candidateId, EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Departments = (await _unitOfWork.Departments.GetAllAsync()).ToList();
            model.Positions = (await _unitOfWork.Positions.GetAllAsync()).ToList();
            ViewBag.Candidate = await _recruitmentService.GetCandidateByIdAsync(candidateId);
            return View(model);
        }

        var employee = new Employee
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Address = model.Address,
            Phone = model.Phone,
            Email = model.Email,
            DepartmentId = model.DepartmentId.Value,
            PositionId = model.PositionId.Value,
            HireDate = model.HireDate,
            ContractType = model.ContractType,
            Status = EmployeeStatus.Actif
        };

        await _recruitmentService.ConvertCandidateToEmployeeAsync(candidateId, employee);

        TempData["Success"] = "Candidat converti en employé avec succès!";
        return RedirectToAction("Details", "Employees", new { id = employee.EmployeeId });
    }
}