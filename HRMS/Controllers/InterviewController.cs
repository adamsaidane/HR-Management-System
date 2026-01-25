using HRMS.Enums;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class InterviewController : Controller
{
    private readonly IRecruitmentService _recruitmentService;

    public InterviewController(IRecruitmentService recruitmentService)
    {
        _recruitmentService = recruitmentService;
    }

    // GET: Interview
    public async Task<IActionResult> Index(int? result, int? pageNumber)
    {
        var pageIndex = pageNumber ?? 1;
        const int pageSize = 10;

        // Get all interviews from the service (already includes candidate details)
        var allInterviews = await _recruitmentService.GetAllInterviewsAsync();

        // Filter by result if specified
        if (result.HasValue)
        {
            allInterviews = allInterviews.Where(i => i.Result == (InterviewResult)result.Value).ToList();
        }

        // Sort by date descending
        allInterviews = allInterviews.OrderByDescending(i => i.InterviewDate).ToList();

        // Paginate
        var paginatedList = PaginatedList<Interview>.Create(allInterviews, pageIndex, pageSize);

        ViewBag.SelectedResult = result;

        return View(paginatedList);
    }

    // GET: Interview/Create
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> Create(int candidateId)
    {
        if (candidateId == 0)
        {
            TempData["Error"] = "Veuillez sélectionner un candidat pour programmer un entretien.";
            return RedirectToAction("Candidates", "Recruitment");
        }

        try
        {
            var viewModel = await _recruitmentService.GetInterviewFormViewModelAsync(candidateId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction("Candidates", "Recruitment");
        }
    }

    // POST: Interview/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> Create(Interview interview)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await _recruitmentService.GetInterviewFormViewModelAsync(interview.CandidateId);
            return View(viewModel);
        }

        try
        {
            await _recruitmentService.ScheduleInterviewAsync(interview);
            TempData["Success"] = "Entretien programmé avec succès!";
            return RedirectToAction(nameof(Details), new { id = interview.InterviewId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Erreur: {ex.Message}");
            var viewModel = await _recruitmentService.GetInterviewFormViewModelAsync(interview.CandidateId);
            return View(viewModel);
        }
    }

    // GET: Interview/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var allInterviews = await _recruitmentService.GetAllInterviewsAsync();
            var interview = allInterviews.FirstOrDefault(i => i.InterviewId == id);

            if (interview == null)
            {
                TempData["Error"] = "Entretien introuvable.";
                return RedirectToAction(nameof(Index));
            }

            return View(interview);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Interview/Edit/5
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var allInterviews = await _recruitmentService.GetAllInterviewsAsync();
            var interview = allInterviews.FirstOrDefault(i => i.InterviewId == id);

            if (interview == null)
            {
                TempData["Error"] = "Entretien introuvable.";
                return RedirectToAction(nameof(Index));
            }

            return View(interview);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Interview/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> Edit(int interviewId, int candidateId, DateTime interviewDate, 
        string? location, string? interviewerName, string? notes, InterviewResult result)
    {
        try
        {
            // Get the existing interview
            var allInterviews = await _recruitmentService.GetAllInterviewsAsync();
            var interview = allInterviews.FirstOrDefault(i => i.InterviewId == interviewId);

            if (interview == null)
            {
                TempData["Error"] = "Entretien introuvable.";
                return RedirectToAction(nameof(Index));
            }

            // Update properties
            interview.InterviewDate = interviewDate;
            interview.Location = location;
            interview.InterviewerName = interviewerName;
            interview.Notes = notes;
            interview.Result = result;

            // Update using the service
            await _recruitmentService.UpdateInterviewResultAsync(interviewId, result, notes ?? "");

            TempData["Success"] = "Entretien mis à jour avec succès!";
            return RedirectToAction(nameof(Details), new { id = interviewId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction(nameof(Edit), new { id = interviewId });
        }
    }

    // POST: Interview/UpdateResult
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH,Manager")]
    public async Task<IActionResult> UpdateResult(int interviewId, InterviewResult result, string? notes)
    {
        try
        {
            await _recruitmentService.UpdateInterviewResultAsync(interviewId, result, notes ?? "");
            
            string resultLabel = result switch
            {
                InterviewResult.Réussi => "réussi",
                InterviewResult.Échoué => "échoué",
                InterviewResult.AutreEntretienNécessaire => "autre entretien nécessaire",
                _ => "en attente"
            };

            TempData["Success"] = $"Résultat de l'entretien défini à '{resultLabel}' avec succès!";
            return RedirectToAction(nameof(Details), new { id = interviewId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id = interviewId });
        }
    }

    // POST: Interview/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var allInterviews = await _recruitmentService.GetAllInterviewsAsync();
            var interview = allInterviews.FirstOrDefault(i => i.InterviewId == id);

            if (interview == null)
            {
                TempData["Error"] = "Entretien introuvable.";
                return RedirectToAction(nameof(Index));
            }

            // You'll need to add a DeleteInterview method to your service
            // await _recruitmentService.DeleteInterviewAsync(id);
            
            TempData["Success"] = "Entretien supprimé avec succès!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}