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
    private readonly IEmployeeService _employeeService;

    public RecruitmentController(
        IRecruitmentService recruitmentService,
        IEmployeeService employeeService)
    {
        _recruitmentService = recruitmentService;
        _employeeService = employeeService;
    }

    // ==================== Offres d'emploi ====================

    public async Task<IActionResult> JobOffers(
        JobOfferStatus? status,
        int? departmentId,
        ContractType? contractType,
        string searchString,
        int? pageNumber)
    {
        var jobOffers = await _recruitmentService.GetJobOffersPaginatedAsync(
            status,
            departmentId,
            contractType,
            searchString,
            pageNumber ?? 1,
            6);

        ViewBag.Departments = await _recruitmentService.GetAllDepartmentsAsync();
        ViewBag.SelectedStatus = status;
        ViewBag.SelectedDepartment = departmentId;
        ViewBag.SelectedContractType = contractType;
        ViewBag.SearchString = searchString;

        return View(jobOffers);
    }

    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CreateJobOffer()
    {
        var viewModel = await _recruitmentService.GetJobOfferFormViewModelAsync();
        if (viewModel.JobOffer == null)
            viewModel.JobOffer = new JobOffer();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> CreateJobOffer(JobOfferFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var formVM = await _recruitmentService.GetJobOfferFormViewModelAsync();
            model.Departments = formVM.Departments;
            model.Positions = formVM.Positions;

            return View(model);
        }

        await _recruitmentService.CreateJobOfferAsync(model.JobOffer);
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
    
    public async Task<IActionResult> Candidates(
        int? jobOfferId,
        CandidateStatus? status,
        string searchString,
        int? pageNumber)
    {
        var candidates = await _recruitmentService.GetCandidatesPaginatedAsync(
            jobOfferId,
            status,
            searchString,
            pageNumber ?? 1,
            15);

        ViewBag.JobOffers = await _recruitmentService.GetAllJobOffersAsync();
        ViewBag.SelectedJobOffer = jobOfferId;
        ViewBag.SelectedStatus = status;
        ViewBag.SearchString = searchString;

        return View(candidates);
    }

    public async Task<IActionResult> CandidateDetails(int id)
    {
        try
        {
            var viewModel = await _recruitmentService.GetCandidateDetailsViewModelAsync(id);
            return View(viewModel);
        }
        catch
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> CreateCandidate()
    {
        var viewModel = await _recruitmentService.GetCandidateFormViewModelAsync();
        return View(viewModel);
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

        await _recruitmentService.CreateCandidateWithCVAsync(model);
        TempData["Success"] = "Candidature enregistrée avec succès!";
        return RedirectToAction(nameof(Candidates));
    }

    // ==================== Entretiens ====================

    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> ScheduleInterview(int candidateId)
    {
        try
        {
            var viewModel = await _recruitmentService.GetInterviewFormViewModelAsync(candidateId);
            return View(viewModel);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> ScheduleInterview(Interview interview)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await _recruitmentService.GetInterviewFormViewModelAsync(interview.CandidateId);
            return View(viewModel);
        }

        await _recruitmentService.ScheduleInterviewAsync(interview);
        TempData["Success"] = "Entretien programmé avec succès!";
        return RedirectToAction(nameof(CandidateDetails), new { id = interview.CandidateId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> UpdateInterviewResult(int interviewId, InterviewResult result, string notes, int candidateId)
    {
        await _recruitmentService.UpdateInterviewResultAsync(interviewId, result, notes);
        
        string resultLabel = result switch
        {
            InterviewResult.Réussi => "réussi",
            InterviewResult.Échoué => "échoué",
            InterviewResult.AutreEntretienNécessaire => "autre entretien nécessaire",
            _ => "en attente"
        };
        
        TempData["Success"] = $"Résultat de l'entretien défini à '{resultLabel}' avec succès!";
        return RedirectToAction(nameof(CandidateDetails), new { id = candidateId });
    }

    // ==================== Conversion Candidat -> Employé ====================

    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> ConvertToEmployee(int candidateId)
    {
        try
        {
            var viewModel = await _recruitmentService.GetConvertToEmployeeViewModelAsync(candidateId);
            var candidate = await _recruitmentService.GetCandidateByIdAsync(candidateId);
            ViewBag.Candidate = candidate;
            return View(viewModel);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> ConvertToEmployee(int candidateId, EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await _recruitmentService.GetConvertToEmployeeViewModelAsync(candidateId);
            return View(viewModel);
        }

        var employee = await _employeeService.CreateEmployeeFromViewModelAsync(model);
        await _recruitmentService.AddCvFromCandidateToEmployeeAsync(candidateId, employee);
        TempData["Success"] = "Candidat converti en employé avec succès!";
        return RedirectToAction("Details", "Employees", new { id = employee.EmployeeId });
    }
}
