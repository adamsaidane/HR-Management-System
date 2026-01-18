using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class SalaryService : ISalaryService
{
    private readonly HRMSDbContext _context;

    public SalaryService(HRMSDbContext context)
    {
        _context = context;
    }

    // Salaires
    public decimal GetCurrentSalary(int employeeId)
    {
        var currentSalary = _context.Salaries
            .Where(s => s.EmployeeId == employeeId &&
                        (s.EndDate == null || s.EndDate > DateTime.Now))
            .OrderByDescending(s => s.EffectiveDate)
            .FirstOrDefault();

        return currentSalary?.BaseSalary ?? 0;
    }

    public IEnumerable<Salary> GetSalaryHistory(int employeeId)
    {
        return _context.Salaries
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveDate)
            .ToList();
    }

    public void AddSalary(Salary salary)
    {
        salary.CreatedDate = DateTime.Now;
        _context.Salaries.Add(salary);
        _context.SaveChanges();
    }

    public void UpdateSalary(int employeeId, decimal newSalary, string justification)
    {
        // Clôturer l'ancien salaire
        var currentSalary = _context.Salaries
            .Where(s => s.EmployeeId == employeeId && s.EndDate == null)
            .OrderByDescending(s => s.EffectiveDate)
            .FirstOrDefault();

        if (currentSalary != null)
        {
            currentSalary.EndDate = DateTime.Now.AddDays(-1);
            _context.Entry(currentSalary).State = EntityState.Modified;
        }

        // Créer le nouveau salaire
        var newSalaryRecord = new Salary
        {
            EmployeeId = employeeId,
            BaseSalary = newSalary,
            EffectiveDate = DateTime.Now,
            Justification = justification
        };

        AddSalary(newSalaryRecord);
    }

    // Primes
    public IEnumerable<Bonus> GetBonusesByEmployee(int employeeId)
    {
        return _context.Bonuses
            .Where(b => b.EmployeeId == employeeId)
            .OrderByDescending(b => b.Date)
            .ToList();
    }

    public void AddBonus(Bonus bonus)
    {
        bonus.CreatedDate = DateTime.Now;
        _context.Bonuses.Add(bonus);
        _context.SaveChanges();
    }

    public decimal GetTotalBonuses(int employeeId, int year)
    {
        return _context.Bonuses
            .Where(b => b.EmployeeId == employeeId && b.Date.Year == year)
            .Sum(b => (decimal?)b.Amount) ?? 0;
    }

    // Avantages
    public IEnumerable<Benefit> GetAllBenefits()
    {
        return _context.Benefits.ToList();
    }

    public IEnumerable<EmployeeBenefit> GetEmployeeBenefits(int employeeId)
    {
        return _context.EmployeeBenefits
            .Include(eb => eb.Benefit)
            .Where(eb => eb.EmployeeId == employeeId &&
                         (eb.EndDate == null || eb.EndDate > DateTime.Now))
            .ToList();
    }

    public void AssignBenefitToEmployee(int employeeId, int benefitId, DateTime startDate)
    {
        var employeeBenefit = new EmployeeBenefit
        {
            EmployeeId = employeeId,
            BenefitId = benefitId,
            StartDate = startDate
        };

        _context.EmployeeBenefits.Add(employeeBenefit);
        _context.SaveChanges();
    }

    public void RemoveBenefitFromEmployee(int employeeBenefitId, DateTime endDate)
    {
        var employeeBenefit = _context.EmployeeBenefits.Find(employeeBenefitId);
        if (employeeBenefit != null)
        {
            employeeBenefit.EndDate = endDate;
            _context.SaveChanges();
        }
    }

    public decimal GetTotalBenefitsValue(int employeeId)
    {
        return _context.EmployeeBenefits
            .Include(eb => eb.Benefit)
            .Where(eb => eb.EmployeeId == employeeId &&
                         (eb.EndDate == null || eb.EndDate > DateTime.Now))
            .Sum(eb => (decimal?)eb.Benefit.Value) ?? 0;
    }

    // Calculs
    public decimal CalculateGrossSalary(int employeeId)
    {
        var baseSalary = GetCurrentSalary(employeeId);
        var benefits = GetTotalBenefitsValue(employeeId);
        return baseSalary + benefits;
    }

    public decimal CalculateTotalCompensation(int employeeId, int year)
    {
        var baseSalary = GetCurrentSalary(employeeId) * 12; // Annuel
        var bonuses = GetTotalBonuses(employeeId, year);
        var benefits = GetTotalBenefitsValue(employeeId) * 12; // Annuel
        return baseSalary + bonuses + benefits;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}