using HRMS.Enums;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class EmployeeService : IEmployeeService
    {
        private readonly HRMSDbContext _context;

        public EmployeeService(HRMSDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .OrderBy(e => e.Matricule)
                .ToList();
        }

        public Employee GetEmployeeById(int id)
        {
            return _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Documents)
                .Include(e => e.Salaries)
                .Include(e => e.Bonuses)
                .Include(e => e.EmployeeBenefits)
                .ThenInclude(eb => eb.Benefit)
                .Include(e => e.EquipmentAssignments)
                .ThenInclude(ea => ea.Equipment)
                .Include(e => e.Promotions)
                .ThenInclude(p => p.OldPosition)
                .Include(e => e.Promotions)
                .ThenInclude(p => p.NewPosition) 
                .FirstOrDefault(e => e.EmployeeId == id);
        }

        public Employee GetEmployeeByMatricule(string matricule)
        {
            return _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefault(e => e.Matricule == matricule);
        }

        public void CreateEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Matricule))
            {
                employee.Matricule = GenerateMatricule();
            }

            employee.CreatedDate = DateTime.Now;
            employee.ModifiedDate = DateTime.Now;

            _context.Employees.Add(employee);
            _context.SaveChanges();
            
            var position = _context.Positions
                .FirstOrDefault(p => p.PositionId == employee.PositionId);

            if (position == null)
                throw new Exception("Position introuvable");

            // Créer le premier enregistrement de salaire
            var salary = new Salary
            {
                EmployeeId = employee.EmployeeId,
                BaseSalary = employee.Position.BaseSalary,
                EffectiveDate = employee.HireDate,
                Justification = "Salaire initial à l'embauche"
            };
            _context.Salaries.Add(salary);
            _context.SaveChanges();
        }

        public void UpdateEmployee(Employee employee)
        {
            employee.ModifiedDate = DateTime.Now;
            _context.Entry(employee).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteEmployee(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Employee> GetActiveEmployees()
        {
            return _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Where(e => e.Status == EmployeeStatus.Actif)
                .OrderBy(e => e.LastName)
                .ToList();
        }

        public IEnumerable<Employee> GetEmployeesByDepartment(int departmentId)
        {
            return _context.Employees
                .Include(e => e.Position)
                .Where(e => e.DepartmentId == departmentId)
                .OrderBy(e => e.LastName)
                .ToList();
        }

        public IEnumerable<Employee> GetEmployeesByStatus(EmployeeStatus status)
        {
            return _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Where(e => e.Status == status)
                .OrderBy(e => e.LastName)
                .ToList();
        }

        public decimal GetEmployeeCurrentSalary(int employeeId)
        {
            var currentSalary = _context.Salaries
                .Where(s => s.EmployeeId == employeeId && 
                           (s.EndDate == null || s.EndDate > DateTime.Now))
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefault();

            return currentSalary?.BaseSalary ?? 0;
        }

        public string GenerateMatricule()
        {
            var year = DateTime.Now.Year;
            var lastMatricule = _context.Employees
                .Where(e => e.Matricule.StartsWith(year.ToString()))
                .OrderByDescending(e => e.Matricule)
                .Select(e => e.Matricule)
                .FirstOrDefault();

            if (lastMatricule == null)
            {
                return $"{year}001";
            }

            var lastNumber = int.Parse(lastMatricule.Substring(4));
            var newNumber = lastNumber + 1;
            return $"{year}{newNumber:D3}";
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }