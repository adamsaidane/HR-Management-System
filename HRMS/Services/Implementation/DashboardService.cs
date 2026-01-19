using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;

namespace HRMS.Service;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
    {
        var stats = new DashboardStatistics();

        // Statistiques Employés
        stats.TotalEmployees = await _unitOfWork.Employees.CountAsync();
        stats.ActiveEmployees = await _unitOfWork.Employees.CountAsync(e => e.Status == EmployeeStatus.Actif);
        stats.SuspendedEmployees = await _unitOfWork.Employees.CountAsync(e => e.Status == EmployeeStatus.Suspendu);
        stats.ResignedEmployees = await _unitOfWork.Employees.CountAsync(e => e.Status == EmployeeStatus.Démissionné);

        // Statistiques Recrutement
        stats.OpenJobOffers = await _unitOfWork.JobOffers.CountAsync(j => j.Status == JobOfferStatus.Ouverte);
        stats.PendingCandidates = await _unitOfWork.Candidates.CountAsync(c => c.Status == CandidateStatus.CandidatureReçue);
        stats.ScheduledInterviews = await _unitOfWork.Interviews.CountAsync(i => 
            i.Result == InterviewResult.EnAttente && i.InterviewDate > DateTime.Now);

        // Statistiques Salaires
        var activeEmployees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);
        var activeSalaries = new List<decimal>();
        
        foreach (var employee in activeEmployees)
        {
            var salary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employee.EmployeeId);
            if (salary != null)
            {
                activeSalaries.Add(salary.BaseSalary);
            }
        }

        stats.TotalMonthlySalary = activeSalaries.Sum();
        stats.AverageSalary = activeSalaries.Any() ? activeSalaries.Average() : 0;

        // Statistiques Équipements
        stats.TotalEquipment = await _unitOfWork.Equipments.CountAsync();
        stats.AssignedEquipment = await _unitOfWork.Equipments.CountAsync(e => e.Status == EquipmentStatus.Affecté);
        stats.AvailableEquipment = await _unitOfWork.Equipments.CountAsync(e => e.Status == EquipmentStatus.Disponible);

        // Répartitions
        stats.EmployeesByDepartment = await GetEmployeesByDepartmentAsync();
        
        var employeesByStatus = await _unitOfWork.Employees.GetAllAsync();
        stats.EmployeesByStatus = employeesByStatus
            .GroupBy(e => e.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var allEquipment = await _unitOfWork.Equipments.GetAllAsync();
        stats.EquipmentByType = allEquipment
            .GroupBy(e => e.EquipmentType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Évolutions
        stats.SalaryEvolutionData = await GetSalaryEvolutionLastYearAsync();
        stats.RecentPromotions = await GetRecentPromotionsAsync(5);

        return stats;
    }

    public async Task<List<SalaryEvolution>> GetSalaryEvolutionLastYearAsync()
    {
        var evolution = new List<SalaryEvolution>();
        var activeEmployees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);

        for (int i = 11; i >= 0; i--)
        {
            var targetDate = DateTime.Now.AddMonths(-i);
            var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            decimal totalSalary = 0;
            int employeeCount = 0;

            foreach (var employee in activeEmployees)
            {
                var salaryHistory = await _unitOfWork.Salaries.GetSalaryHistoryAsync(employee.EmployeeId);
                var relevantSalary = salaryHistory
                    .Where(s => s.EffectiveDate <= monthEnd &&
                               (s.EndDate == null || s.EndDate >= monthStart))
                    .OrderByDescending(s => s.EffectiveDate)
                    .FirstOrDefault();

                if (relevantSalary != null)
                {
                    totalSalary += relevantSalary.BaseSalary;
                    employeeCount++;
                }
            }

            evolution.Add(new SalaryEvolution
            {
                Month = monthStart.ToString("MMM yyyy"),
                TotalSalary = totalSalary,
                EmployeeCount = employeeCount
            });
        }

        return evolution;
    }

    public async Task<List<RecentPromotion>> GetRecentPromotionsAsync(int count = 10)
    {
        var promotions = await _unitOfWork.Promotions.GetRecentPromotionsAsync(count);
        
        return promotions.Select(p => new RecentPromotion
        {
            EmployeeName = p.Employee.FirstName + " " + p.Employee.LastName,
            OldPosition = p.OldPosition.Title,
            NewPosition = p.NewPosition.Title,
            PromotionDate = p.PromotionDate,
            SalaryIncrease = p.NewSalary - p.OldSalary
        }).ToList();
    }

    public async Task<Dictionary<string, int>> GetEmployeesByDepartmentAsync()
    {
        var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);
        var departments = await _unitOfWork.Departments.GetAllAsync();
        
        var result = new Dictionary<string, int>();
        foreach (var dept in departments)
        {
            var count = employees.Count(e => e.DepartmentId == dept.DepartmentId);
            result[dept.Name] = count;
        }
        
        return result;
    }

    public async Task<Dictionary<string, decimal>> GetDepartmentSalaryDistributionAsync()
    {
        var result = new Dictionary<string, decimal>();
        var departments = await _unitOfWork.Departments.GetAllAsync();

        foreach (var dept in departments)
        {
            var deptEmployees = await _unitOfWork.Employees
                .FindAsync(e => e.DepartmentId == dept.DepartmentId && e.Status == EmployeeStatus.Actif);

            decimal deptSalary = 0;
            foreach (var employee in deptEmployees)
            {
                var salary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(employee.EmployeeId);
                if (salary != null)
                {
                    deptSalary += salary.BaseSalary;
                }
            }

            result[dept.Name] = deptSalary;
        }

        return result;
    }
}