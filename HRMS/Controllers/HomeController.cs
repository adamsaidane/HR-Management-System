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
            TotalEquipment = stats.TotalEquipment,
            EmployeesByAgeRange = stats.EmployeesByAgeRange,
            EmployeesByContractType = stats.EmployeesByContractType,
            EmployeesBySeniority = stats.EmployeesBySeniority,
            EmployeesByGender = stats.EmployeesByGender,
            CandidatesByStage = stats.CandidatesByStage,
            JobOffersByDepartment = stats.JobOffersByDepartment,
            SalaryByDepartment = stats.SalaryByDepartment,
            EquipmentByStatus = stats.EquipmentByStatus,
            HiringTrendByMonth = stats.HiringTrendByMonth,
            BonusesByDepartment = stats.BonusesByDepartment,
            BonusesByType = stats.BonusesByType,
            DocumentsByType = stats.DocumentsByType,
            SalaryDistribution = stats.SalaryDistribution,
            PromotionsByDepartment = stats.PromotionsByDepartment,
            InterviewSuccessRate = stats.InterviewSuccessRate,
            EquipmentByType = stats.EquipmentByType,
            BenefitUtilization = stats.BenefitUtilization,
            EmployeeGrowthByYear = stats.EmployeeGrowthByYear,
            CandidatesApplicationTrend = stats.CandidatesApplicationTrend,
            AverageSalaryByDepartment = stats.AverageSalaryByDepartment,
            BonusTrendByMonth = stats.BonusTrendByMonth,
            EquipmentPurchaseByYear = stats.EquipmentPurchaseByYear,
            EquipmentAssignmentsByDepartment = stats.EquipmentAssignmentsByDepartment,
            ActiveJobOffersByContractType = stats.ActiveJobOffersByContractType,
            InterviewsScheduledVsCompleted = stats.InterviewsScheduledVsCompleted,
            PromotionTrendByMonth = stats.PromotionTrendByMonth,
            SalaryIncreaseTrend = stats.SalaryIncreaseTrend,
            EmployeesByAgeAndGender = stats.EmployeesByAgeAndGender,
            BenefitCostByDepartment = stats.BenefitCostByDepartment,
            EmployeeRetentionByYear = stats.EmployeeRetentionByYear,
            BenefitsByType = stats.BenefitsByType,
            SalaryGrowthByDepartment = stats.SalaryGrowthByDepartment,
            AverageBonusByDepartment = stats.AverageBonusByDepartment,
            InterviewsByMonth = stats.InterviewsByMonth,
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