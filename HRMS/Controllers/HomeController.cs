using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HRMS.Models;
using HRMS.Service;
using HRMS.ViewModels;
using Microsoft.AspNetCore.Authorization;

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
    public ActionResult Index()
    {
        var stats = _dashboardService.GetDashboardStatistics();

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