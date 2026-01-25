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

        stats.TotalEquipment = await _unitOfWork.Equipments.CountAsync();
        stats.AssignedEquipment = await _unitOfWork.Equipments.CountAsync(e => e.Status == EquipmentStatus.Affecté);
        stats.AvailableEquipment = await _unitOfWork.Equipments.CountAsync(e => e.Status == EquipmentStatus.Disponible);

        stats.EmployeesByDepartment = await GetEmployeesByDepartmentAsync();
        stats.EmployeesByStatus = await GetEmployeesByStatusAsync();
        stats.EmployeesByGender = await GetEmployeesByGenderAsync();
        stats.EmployeesByAgeRange = await GetEmployeesByAgeRangeAsync();
        stats.EmployeesByContractType = await GetEmployeesByContractTypeAsync();
        stats.EmployeesBySeniority = await GetEmployeesBySeniorityAsync();
        stats.CandidatesByStage = await GetCandidatesByStageAsync();
        stats.JobOffersByDepartment = await GetJobOffersByDepartmentAsync();
        stats.SalaryByDepartment = await GetSalaryByDepartmentAsync();
        stats.EquipmentByStatus = await GetEquipmentByStatusAsync();
        
        stats.HiringTrendByMonth = await GetHiringTrendByMonthAsync();
        stats.TerminationTrendByMonth = await GetTerminationTrendByMonthAsync();
        stats.BonusesByDepartment = await GetBonusesByDepartmentAsync();
        stats.BonusesByType = await GetBonusesByTypeAsync();
        stats.DocumentsByType = await GetDocumentsByTypeAsync();
        stats.SalaryDistribution = await GetSalaryDistributionAsync();
        stats.PromotionsByDepartment = await GetPromotionsByDepartmentAsync();
        stats.InterviewSuccessRate = await GetInterviewSuccessRateAsync();
        stats.AverageSalaryByPosition = await GetAverageSalaryByPositionAsync();
        stats.EquipmentByType = await GetEquipmentByTypeAsync();
        stats.BenefitUtilization = await GetBenefitUtilizationAsync();
        stats.EmployeeGrowthByYear = await GetEmployeeGrowthByYearAsync();
        stats.TurnoverRate = await GetTurnoverRateAsync();
        stats.CandidatesApplicationTrend = await GetCandidatesApplicationTrendAsync();
        
        // Évolutions
        stats.SalaryEvolutionData = await GetSalaryEvolutionLastYearAsync();
        stats.RecentPromotions = await GetRecentPromotionsAsync(5);
        
        return stats;
    }

    private async Task<List<SalaryEvolution>> GetSalaryEvolutionLastYearAsync()
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

    private async Task<List<RecentPromotion>> GetRecentPromotionsAsync(int count = 10)
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

    private async Task<Dictionary<string, int>> GetEmployeesByDepartmentAsync()
    {
        var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);
        var departments = await _unitOfWork.Departments.GetAllAsync();
        
        var result = new Dictionary<string, int>();
        foreach (var dept in departments)
        {
            var count = employees.Count(e => e.DepartmentId == dept.DepartmentId);
            if (count > 0)
                result[dept.Name] = count;
        }
        
        return result;
    }

    private async Task<Dictionary<string, int>> GetEmployeesByStatusAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        return employees.GroupBy(e => e.Status.ToString()).ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetEmployeesByGenderAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        return employees.GroupBy(e => e.Gender.ToString()).ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetEmployeesByAgeRangeAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var today = DateTime.Today;

        return employees
            .Select(e => today.Year - e.DateOfBirth.Year)
            .GroupBy(age =>
                age < 25 ? "< 25" :
                age <= 35 ? "25-35" :
                age <= 45 ? "36-45" : "45+")
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetEmployeesByContractTypeAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        return employees.GroupBy(e => e.ContractType.ToString()).ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetEmployeesBySeniorityAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var today = DateTime.Today;

        return employees
            .Select(e => today.Year - e.HireDate.Year)
            .GroupBy(y =>
                y < 1 ? "< 1 an" :
                y <= 3 ? "1-3 ans" :
                y <= 5 ? "3-5 ans" : "5+ ans")
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetCandidatesByStageAsync()
    {
        var candidates = await _unitOfWork.Candidates.GetAllAsync();
        return candidates.GroupBy(c => c.Status.ToString()).ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetJobOffersByDepartmentAsync()
    {
        var offers = await _unitOfWork.JobOffers.GetAllAsync();
        return offers
            .Where(o => o.Department != null)
            .GroupBy(o => o.Department.Name)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, decimal>> GetSalaryByDepartmentAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var result = new Dictionary<string, decimal>();

        foreach (var dept in employees.Where(e => e.Department != null).GroupBy(e => e.Department.Name))
        {
            decimal total = 0;
            foreach (var emp in dept)
            {
                var salary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(emp.EmployeeId);
                if (salary != null) total += salary.BaseSalary;
            }
            if (total > 0)
                result.Add(dept.Key, total);
        }
        return result;
    }

    private async Task<Dictionary<string, int>> GetEquipmentByStatusAsync()
    {
        var eq = await _unitOfWork.Equipments.GetAllAsync();
        return eq.GroupBy(e => e.Status).ToDictionary(g => g.Key.ToString(), g => g.Count());
    }

    // NEW ANALYTICS METHODS WITH NULL CHECKS
    private async Task<Dictionary<string, int>> GetHiringTrendByMonthAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var result = new Dictionary<string, int>();

        for (int i = 11; i >= 0; i--)
        {
            var targetDate = DateTime.Now.AddMonths(-i);
            var monthKey = targetDate.ToString("MMM yyyy");
            var count = employees.Count(e => e.HireDate.Year == targetDate.Year && e.HireDate.Month == targetDate.Month);
            result[monthKey] = count;
        }
        return result;
    }

    private async Task<Dictionary<string, int>> GetTerminationTrendByMonthAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var result = new Dictionary<string, int>();

        for (int i = 11; i >= 0; i--)
        {
            var targetDate = DateTime.Now.AddMonths(-i);
            var monthKey = targetDate.ToString("MMM yyyy");
            var count = employees.Count(e => e.EndDate.HasValue && 
                e.EndDate.Value.Year == targetDate.Year && 
                e.EndDate.Value.Month == targetDate.Month);
            result[monthKey] = count;
        }
        return result;
    }

    private async Task<Dictionary<string, decimal>> GetBonusesByDepartmentAsync()
    {
        var bonuses = await _unitOfWork.Bonuses.GetAllAsync();
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var result = new Dictionary<string, decimal>();

        var deptGroups = bonuses
            .Join(employees.Where(e => e.Department != null), 
                b => b.EmployeeId, 
                e => e.EmployeeId, 
                (b, e) => new { b.Amount, e.Department.Name })
            .GroupBy(x => x.Name);

        foreach (var group in deptGroups)
        {
            result[group.Key] = group.Sum(x => x.Amount);
        }
        return result;
    }

    private async Task<Dictionary<string, int>> GetBonusesByTypeAsync()
    {
        var bonuses = await _unitOfWork.Bonuses.GetAllAsync();
        return bonuses
            .Where(b => !string.IsNullOrEmpty(b.BonusType))
            .GroupBy(b => b.BonusType)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetDocumentsByTypeAsync()
    {
        var documents = await _unitOfWork.Documents.GetAllAsync();
        return documents
            .Where(d => !string.IsNullOrEmpty(d.DocumentType))
            .GroupBy(d => d.DocumentType)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, decimal>> GetSalaryDistributionAsync()
    {
        var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);
        var result = new Dictionary<string, decimal>();
        
        var ranges = new[] { "0-1000", "1000-2000", "2000-3000", "3000-5000", "5000+" };
        foreach (var range in ranges) result[range] = 0;

        foreach (var emp in employees)
        {
            var salary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(emp.EmployeeId);
            if (salary != null)
            {
                var amount = salary.BaseSalary;
                if (amount < 1000) result["0-1000"]++;
                else if (amount < 2000) result["1000-2000"]++;
                else if (amount < 3000) result["2000-3000"]++;
                else if (amount < 5000) result["3000-5000"]++;
                else result["5000+"]++;
            }
        }
        return result;
    }

    private async Task<Dictionary<string, int>> GetPromotionsByDepartmentAsync()
    {
        var promotions = await _unitOfWork.Promotions.GetAllAsync();
        var employees = await _unitOfWork.Employees.GetAllAsync();
        
        return promotions
            .Join(employees.Where(e => e.Department != null), 
                p => p.EmployeeId, 
                e => e.EmployeeId, 
                (p, e) => e.Department.Name)
            .GroupBy(name => name)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetInterviewSuccessRateAsync()
    {
        var interviews = await _unitOfWork.Interviews.GetAllAsync();
        return interviews
            .Where(i => i.Result.HasValue)
            .GroupBy(i => i.Result.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, decimal>> GetAverageSalaryByPositionAsync()
    {
        var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Actif);
        var result = new Dictionary<string, List<decimal>>();

        foreach (var emp in employees)
        {
            // Check if Position is not null
            if (emp.Position == null || string.IsNullOrEmpty(emp.Position.Title))
                continue;

            var salary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(emp.EmployeeId);
            if (salary != null)
            {
                if (!result.ContainsKey(emp.Position.Title))
                    result[emp.Position.Title] = new List<decimal>();
                result[emp.Position.Title].Add(salary.BaseSalary);
            }
        }

        return result
            .Where(kvp => kvp.Value.Any())
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average());
    }

    private async Task<Dictionary<string, int>> GetEquipmentByTypeAsync()
    {
        var equipment = await _unitOfWork.Equipments.GetAllAsync();
        return equipment
            .Where(e => !string.IsNullOrEmpty(e.EquipmentType))
            .GroupBy(e => e.EquipmentType)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<string, int>> GetBenefitUtilizationAsync()
    {
        var empBenefits = await _unitOfWork.EmployeeBenefits.GetAllAsync();
        var benefits = await _unitOfWork.Benefits.GetAllAsync();
        
        return empBenefits
            .Join(benefits.Where(b => !string.IsNullOrEmpty(b.BenefitType)), 
                eb => eb.BenefitId, 
                b => b.BenefitId, 
                (eb, b) => b.BenefitType)
            .GroupBy(type => type)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<Dictionary<int, int>> GetEmployeeGrowthByYearAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var currentYear = DateTime.Now.Year;
        var result = new Dictionary<int, int>();

        for (int year = currentYear - 5; year <= currentYear; year++)
        {
            result[year] = employees.Count(e => e.HireDate.Year <= year && 
                (!e.EndDate.HasValue || e.EndDate.Value.Year > year));
        }
        return result;
    }

    private async Task<Dictionary<string, decimal>> GetTurnoverRateAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var result = new Dictionary<string, decimal>();

        for (int i = 11; i >= 0; i--)
        {
            var targetDate = DateTime.Now.AddMonths(-i);
            var monthKey = targetDate.ToString("MMM yyyy");
            var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var activeCount = employees.Count(e => e.HireDate <= monthEnd && 
                (!e.EndDate.HasValue || e.EndDate.Value > monthStart));
            var terminatedCount = employees.Count(e => e.EndDate.HasValue && 
                e.EndDate.Value >= monthStart && e.EndDate.Value <= monthEnd);

            result[monthKey] = activeCount > 0 ? (decimal)terminatedCount / activeCount * 100 : 0;
        }
        return result;
    }

    private async Task<Dictionary<string, int>> GetCandidatesApplicationTrendAsync()
    {
        var candidates = await _unitOfWork.Candidates.GetAllAsync();
        var result = new Dictionary<string, int>();

        for (int i = 11; i >= 0; i--)
        {
            var targetDate = DateTime.Now.AddMonths(-i);
            var monthKey = targetDate.ToString("MMM yyyy");
            var count = candidates.Count(c => c.ApplicationDate.Year == targetDate.Year && 
                c.ApplicationDate.Month == targetDate.Month);
            result[monthKey] = count;
        }
        return result;
    }
}