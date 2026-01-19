using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET: Home/Index - Dashboard
    public async Task<ActionResult> Index()
    {
        var stats = await _dashboardService.GetDashboardStatisticsAsync();

        var viewModel = new DashboardViewModel
        {
            TotalEmployees = stats.TotalEmployees,
            ActiveEmployees = stats.ActiveEmployees,
            OpenJobOffers = stats.OpenJobOffers,
            PendingCandidates = stats.PendingCandidates,
            TotalMonthlySalary = stats.TotalMonthlySalary,
            AverageSalary = stats.AverageSalary,
            AssignedEquipment = stats.AssignedEquipment,
            AvailableEquipment = stats.AvailableEquipment,
            EmployeesByDepartment = stats.EmployeesByDepartment,
            EmployeesByStatus = stats.EmployeesByStatus,
            SalaryEvolution = stats.SalaryEvolutionData,
            RecentPromotions = stats.RecentPromotions,
            TotalEquipment = stats.TotalEquipment
        };

        return View(viewModel);
    }

    // GET: Home/About
    public ActionResult About()
    {
        ViewBag.Message = "Système de Gestion des Ressources Humaines";
        return View();
    }
}