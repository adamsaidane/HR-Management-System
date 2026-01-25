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
        stats.EmployeesByStatus = await GetEmployeesByStatusAsync();
        stats.EmployeesByGender = await GetEmployeesByGenderAsync();
        stats.EmployeesByAgeRange = await GetEmployeesByAgeRangeAsync();
        stats.EmployeesByContractType = await GetEmployeesByContractTypeAsync();
        stats.EmployeesBySeniority = await GetEmployeesBySeniorityAsync();

        stats.CandidatesByStage = await GetCandidatesByStageAsync();
        stats.JobOffersByDepartment = await GetJobOffersByDepartmentAsync();

        stats.SalaryByDepartment = await GetSalaryByDepartmentAsync();

        stats.EquipmentByStatus = await GetEquipmentByStatusAsync();
        
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
    private async Task<Dictionary<string, int>> GetEmployeesByStatusAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();

        return employees
            .GroupBy(e => e.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }
    private async Task<Dictionary<string, int>> GetEmployeesByGenderAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();

        return employees
            .GroupBy(e => e.Gender.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
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
                age <= 45 ? "36-45" :
                "45+")
            .ToDictionary(g => g.Key, g => g.Count());
    }
    private async Task<Dictionary<string, int>> GetEmployeesByContractTypeAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();

        return employees
            .GroupBy(e => e.ContractType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
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
                y <= 5 ? "3-5 ans" :
                "5+ ans")
            .ToDictionary(g => g.Key, g => g.Count());
    }
    private async Task<Dictionary<string, int>> GetCandidatesByStageAsync()
    {
        var candidates = await _unitOfWork.Candidates.GetAllAsync();

        return candidates
            .GroupBy(c => c.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }
    private async Task<Dictionary<string, int>> GetJobOffersByDepartmentAsync()
    {
        var offers = await _unitOfWork.JobOffers.GetAllAsync();

        return offers
            .GroupBy(o => o.Department.Name)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    private async Task<Dictionary<string, decimal>> GetSalaryByDepartmentAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var result = new Dictionary<string, decimal>();

        foreach (var dept in employees.GroupBy(e => e.Department.Name))
        {
            decimal total = 0;
            foreach (var emp in dept)
            {
                var salary = await _unitOfWork.Salaries.GetCurrentSalaryAsync(emp.EmployeeId);
                if (salary != null)
                    total += salary.BaseSalary;
            }
            result.Add(dept.Key, total);
        }

        return result;
    }
    private async Task<Dictionary<string, int>> GetEquipmentByStatusAsync()
    {
        var eq = await _unitOfWork.Equipments.GetAllAsync();

        return eq
            .GroupBy(e => e.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());
    }

}