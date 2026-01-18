namespace HRMS.Controllers;

using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class RecruitmentController : Controller
{
    private readonly IRecruitmentService _recruitmentService;
    private readonly HRMSDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public RecruitmentController(
        IRecruitmentService recruitmentService,
        HRMSDbContext context,
        IWebHostEnvironment environment)
    {
        _recruitmentService = recruitmentService;
        _context = context;
        _environment = environment;
    }

    // ==================== Offres d'emploi ====================

    public IActionResult JobOffers()
    {
        var jobOffers = _recruitmentService.GetAllJobOffers();
        return View(jobOffers);
    }

    [Authorize(Roles = "AdminRH")]
    public IActionResult CreateJobOffer()
    {
        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Positions = _context.Positions.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult CreateJobOffer(JobOffer model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _recruitmentService.CreateJobOffer(model);
        TempData["Success"] = "Offre d'emploi créée avec succès!";
        return RedirectToAction(nameof(JobOffers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult CloseJobOffer(int id)
    {
        _recruitmentService.CloseJobOffer(id);
        TempData["Success"] = "Offre fermée avec succès!";
        return RedirectToAction(nameof(JobOffers));
    }

    // ==================== Candidats ====================

    public IActionResult Candidates(int? jobOfferId)
    {
        var candidates = jobOfferId.HasValue
            ? _recruitmentService.GetCandidatesByJobOffer(jobOfferId.Value)
            : _recruitmentService.GetAllCandidates();

        ViewBag.JobOffers = _context.JobOffers.ToList();
        ViewBag.SelectedJobOffer = jobOfferId;
        return View(candidates);
    }

    public IActionResult CandidateDetails(int id)
    {
        var candidate = _recruitmentService.GetCandidateById(id);
        if (candidate == null)
            return NotFound();

        ViewBag.Interviews = _recruitmentService.GetInterviewsByCandidate(id);
        return View(candidate);
    }

    public IActionResult CreateCandidate()
    {
        return View(new CandidateFormViewModel
        {
            JobOffers = _recruitmentService.GetActiveJobOffers()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateCandidate(CandidateFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors) 
            {
                Console.WriteLine(error.ErrorMessage);
            }
            model.JobOffers = _recruitmentService.GetActiveJobOffers();
            return View(model);
        }

        var candidate = new Candidate
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            JobOfferId = model.JobOfferId.Value
        };

        if (model.CVFile != null && model.CVFile.Length > 0)
        {
            var folder = Path.Combine(_environment.WebRootPath, "Content/CVs");
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(model.CVFile.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            model.CVFile.CopyTo(stream);

            candidate.CVPath = "/Content/CVs/" + fileName;
        }

        _recruitmentService.CreateCandidate(candidate);
        TempData["Success"] = "Candidature enregistrée avec succès!";
        return RedirectToAction(nameof(Candidates));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public IActionResult UpdateCandidateStatus(int candidateId, CandidateStatus status)
    {
        _recruitmentService.UpdateCandidateStatus(candidateId, status);
        TempData["Success"] = "Statut mis à jour!";
        return RedirectToAction(nameof(CandidateDetails), new { id = candidateId });
    }

    // ==================== Entretiens ====================

    [Authorize(Roles = "AdminRH,Manager")]
    public IActionResult ScheduleInterview(int candidateId)
    {
        var candidate = _recruitmentService.GetCandidateById(candidateId);
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
    public IActionResult ScheduleInterview(Interview interview)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Candidate = _recruitmentService.GetCandidateById(interview.CandidateId);
            return View(interview);
        }

        _recruitmentService.ScheduleInterview(interview);
        TempData["Success"] = "Entretien programmé avec succès!";
        return RedirectToAction(nameof(CandidateDetails), new { id = interview.CandidateId });
    }

    // ==================== Conversion Candidat -> Employé ====================

    [Authorize(Roles = "AdminRH")]
    public IActionResult ConvertToEmployee(int candidateId)
    {
        var candidate = _recruitmentService.GetCandidateById(candidateId);
        if (candidate == null)
            return NotFound();

        ViewBag.Candidate = candidate;

        return View(new EmployeeFormViewModel
        {
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            HireDate = DateTime.Today,
            Departments = _context.Departments.ToList(),
            Positions = _context.Positions.ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public IActionResult ConvertToEmployee(int candidateId, EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Departments = _context.Departments.ToList();
            model.Positions = _context.Positions.ToList();
            ViewBag.Candidate = _recruitmentService.GetCandidateById(candidateId);
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
            DepartmentId = model.DepartmentId,
            PositionId = model.PositionId,
            HireDate = model.HireDate,
            ContractType = model.ContractType,
            Status = EmployeeStatus.Actif
        };

        _recruitmentService.ConvertCandidateToEmployee(candidateId, employee);

        TempData["Success"] = "Candidat converti en employé avec succès!";
        return RedirectToAction("Details", "Employees", new { id = employee.EmployeeId });
    }
}
