using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class PromotionService : IPromotionService
    {
        private readonly HRMSDbContext _context;
        private readonly ISalaryService _salaryService;

        public PromotionService(HRMSDbContext context, ISalaryService salaryService)
        {
            _context = context;
            _salaryService = salaryService;
        }

        public IEnumerable<Promotion> GetAllPromotions()
        {
            return _context.Promotions
                .Include(p => p.Employee)
                .Include(p => p.OldPosition)
                .Include(p => p.NewPosition)
                .OrderByDescending(p => p.PromotionDate)
                .ToList();
        }

        public IEnumerable<Promotion> GetEmployeePromotions(int employeeId)
        {
            return _context.Promotions
                .Include(p => p.OldPosition)
                .Include(p => p.NewPosition)
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.PromotionDate)
                .ToList();
        }

        public void CreatePromotion(Promotion promotion)
        {
            promotion.CreatedDate = DateTime.Now;
            _context.Promotions.Add(promotion);
            _context.SaveChanges();
        }

        public void ProcessPromotion(int employeeId, int newPositionId, decimal newSalary, string justification)
        {
            var employee = _context.Employees.Find(employeeId);
            if (employee == null)
                throw new Exception("Employé introuvable");

            var currentSalary = _salaryService.GetCurrentSalary(employeeId);

            // Créer l'enregistrement de promotion
            var promotion = new Promotion
            {
                EmployeeId = employeeId,
                OldPositionId = employee.PositionId,
                NewPositionId = newPositionId,
                OldSalary = currentSalary,
                NewSalary = newSalary,
                PromotionDate = DateTime.Now,
                Justification = justification
            };

            CreatePromotion(promotion);

            // Mettre à jour l'employé
            employee.PositionId = newPositionId;
            employee.ModifiedDate = DateTime.Now;
            _context.Entry(employee).State = EntityState.Modified;

            // Mettre à jour le salaire
            _salaryService.UpdateSalary(employeeId, newSalary, $"Promotion: {justification}");

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }