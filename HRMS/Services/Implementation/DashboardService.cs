using HRMS.Enums;
using HRMS.Models;
using HRMS.ViewModels;

namespace HRMS.Service;

public class DashboardService : IDashboardService
    {
        private readonly HRMSDbContext _context;

        public DashboardService(HRMSDbContext context)
        {
            _context = context;
        }

        public DashboardStatistics GetDashboardStatistics()
        {
            var stats = new DashboardStatistics();

            // Statistiques Employés
            stats.TotalEmployees = _context.Employees.Count();
            stats.ActiveEmployees = _context.Employees.Count(e => e.Status == EmployeeStatus.Actif);
            stats.SuspendedEmployees = _context.Employees.Count(e => e.Status == EmployeeStatus.Suspendu);
            stats.ResignedEmployees = _context.Employees.Count(e => e.Status == EmployeeStatus.Démissionné);

            // Statistiques Recrutement
            stats.OpenJobOffers = _context.JobOffers.Count(j => j.Status == JobOfferStatus.Ouverte);
            stats.PendingCandidates = _context.Candidates.Count(c => c.Status == CandidateStatus.CandidatureReçue);
            stats.ScheduledInterviews = _context.Interviews.Count(i => i.Result == InterviewResult.EnAttente && i.InterviewDate > DateTime.Now);

            // Statistiques Salaires
            var activeSalaries = _context.Salaries
                .Where(s => _context.Employees.Any(e => e.EmployeeId == s.EmployeeId && e.Status == EmployeeStatus.Actif) 
                           && (s.EndDate == null || s.EndDate > DateTime.Now))
                .GroupBy(s => s.EmployeeId)
                .Select(g => g.OrderByDescending(s => s.EffectiveDate).FirstOrDefault())
                .ToList();

            stats.TotalMonthlySalary = activeSalaries.Sum(s => s.BaseSalary);
            stats.AverageSalary = activeSalaries.Any() ? activeSalaries.Average(s => s.BaseSalary) : 0;

            // Statistiques Équipements
            stats.TotalEquipment = _context.Equipments.Count();
            stats.AssignedEquipment = _context.Equipments.Count(e => e.Status == EquipmentStatus.Affecté);
            stats.AvailableEquipment = _context.Equipments.Count(e => e.Status == EquipmentStatus.Disponible);

            // Répartitions
            stats.EmployeesByDepartment = GetEmployeesByDepartment();
            stats.EmployeesByStatus = _context.Employees
                .GroupBy(e => e.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            
            stats.EquipmentByType = _context.Equipments
                .GroupBy(e => e.EquipmentType)
                .ToDictionary(g => g.Key, g => g.Count());

            // Évolutions
            stats.SalaryEvolutionData = GetSalaryEvolutionLastYear();
            stats.RecentPromotions = GetRecentPromotions(5);

            return stats;
        }

        public List<SalaryEvolution> GetSalaryEvolutionLastYear()
        {
            var oneYearAgo = DateTime.Now.AddYears(-1);
            var evolution = new List<SalaryEvolution>();

            for (int i = 11; i >= 0; i--)
            {
                var targetDate = DateTime.Now.AddMonths(-i);
                var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var activeSalaries = _context.Salaries
                    .Where(s => s.EffectiveDate <= monthEnd &&
                               (s.EndDate == null || s.EndDate >= monthStart) &&
                               _context.Employees.Any(e => e.EmployeeId == s.EmployeeId && e.Status == EmployeeStatus.Actif))
                    .GroupBy(s => s.EmployeeId)
                    .Select(g => g.OrderByDescending(s => s.EffectiveDate).FirstOrDefault())
                    .ToList();

                evolution.Add(new SalaryEvolution
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    TotalSalary = activeSalaries.Sum(s => s.BaseSalary),
                    EmployeeCount = activeSalaries.Count()
                });
            }

            return evolution;
        }

        public List<RecentPromotion> GetRecentPromotions(int count = 10)
        {
            return _context.Promotions
                .OrderByDescending(p => p.PromotionDate)
                .Take(count)
                .Select(p => new RecentPromotion
                {
                    EmployeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                    OldPosition = p.OldPosition.Title,
                    NewPosition = p.NewPosition.Title,
                    PromotionDate = p.PromotionDate,
                    SalaryIncrease = p.NewSalary - p.OldSalary
                })
                .ToList();
        }

        public Dictionary<string, int> GetEmployeesByDepartment()
        {
            return _context.Employees
                .Where(e => e.Status == EmployeeStatus.Actif)
                .GroupBy(e => e.Department.Name)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public Dictionary<string, decimal> GetDepartmentSalaryDistribution()
        {
            var result = new Dictionary<string, decimal>();

            var departments = _context.Departments.ToList();
            foreach (var dept in departments)
            {
                var deptSalary = _context.Employees
                    .Where(e => e.DepartmentId == dept.DepartmentId && e.Status == EmployeeStatus.Actif)
                    .Join(_context.Salaries.Where(s => s.EndDate == null || s.EndDate > DateTime.Now),
                          e => e.EmployeeId,
                          s => s.EmployeeId,
                          (e, s) => s.BaseSalary)
                    .Sum();

                result[dept.Name] = deptSalary;
            }

            return result;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }