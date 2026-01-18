using HRMS.Enums;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Service;

public class EquipmentService : IEquipmentService
    {
        private readonly HRMSDbContext _context;
        
        public EquipmentService(HRMSDbContext context)
        {
            _context = context;
        }

        // Equipment CRUD
        public IEnumerable<Equipment> GetAllEquipment()
        {
            return _context.Equipments
                .OrderBy(e => e.EquipmentType)
                .ThenBy(e => e.Brand)
                .ToList();
        }

        public IEnumerable<Equipment> GetAvailableEquipment()
        {
            return _context.Equipments
                .Where(e => e.Status == EquipmentStatus.Disponible)
                .OrderBy(e => e.EquipmentType)
                .ToList();
        }

        public Equipment GetEquipmentById(int id)
        {
            return _context.Equipments
                .Include(e => e.EquipmentAssignments.Select(ea => ea.Employee))
                .FirstOrDefault(e => e.EquipmentId == id);
        }

        public void CreateEquipment(Equipment equipment)
        {
            equipment.CreatedDate = DateTime.Now;
            equipment.ModifiedDate = DateTime.Now;
            _context.Equipments.Add(equipment);
            _context.SaveChanges();
        }

        public void UpdateEquipment(Equipment equipment)
        {
            equipment.ModifiedDate = DateTime.Now;
            _context.Entry(equipment).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteEquipment(int id)
        {
            var equipment = _context.Equipments.Find(id);
            if (equipment != null)
            {
                _context.Equipments.Remove(equipment);
                _context.SaveChanges();
            }
        }

        // Assignments
        public IEnumerable<EquipmentAssignment> GetEmployeeEquipment(int employeeId)
        {
            return _context.EquipmentAssignments
                .Include(ea => ea.Equipment)
                .Where(ea => ea.EmployeeId == employeeId)
                .OrderByDescending(ea => ea.AssignmentDate)
                .ToList();
        }

        public IEnumerable<EquipmentAssignment> GetEquipmentHistory(int equipmentId)
        {
            return _context.EquipmentAssignments
                .Include(ea => ea.Employee)
                .Where(ea => ea.EquipmentId == equipmentId)
                .OrderByDescending(ea => ea.AssignmentDate)
                .ToList();
        }

        public void AssignEquipment(int equipmentId, int employeeId, string condition, string notes)
        {
            var assignment = new EquipmentAssignment
            {
                EquipmentId = equipmentId,
                EmployeeId = employeeId,
                AssignmentDate = DateTime.Now,
                Condition = condition,
                Notes = notes
            };

            _context.EquipmentAssignments.Add(assignment);

            // Mettre à jour le statut de l'équipement
            var equipment = _context.Equipments.Find(equipmentId);
            if (equipment != null)
            {
                equipment.Status = EquipmentStatus.Affecté;
                equipment.ModifiedDate = DateTime.Now;
            }

            _context.SaveChanges();
        }

        public void ReturnEquipment(int assignmentId, DateTime returnDate, string condition)
        {
            var assignment = _context.EquipmentAssignments
                .Include(ea => ea.Equipment)
                .FirstOrDefault(ea => ea.AssignmentId == assignmentId);

            if (assignment != null)
            {
                assignment.ReturnDate = returnDate;
                assignment.Condition = condition;

                // Mettre à jour le statut de l'équipement
                if (assignment.Equipment != null)
                {
                    assignment.Equipment.Status = EquipmentStatus.Disponible;
                    assignment.Equipment.ModifiedDate = DateTime.Now;
                }

                _context.SaveChanges();
            }
        }

        // Statistics
        public int GetTotalEquipment()
        {
            return _context.Equipments.Count();
        }

        public int GetAssignedEquipmentCount()
        {
            return _context.Equipments
                .Count(e => e.Status == EquipmentStatus.Affecté);
        }

        public int GetAvailableEquipmentCount()
        {
            return _context.Equipments
                .Count(e => e.Status == EquipmentStatus.Disponible);
        }

        public Dictionary<string, int> GetEquipmentByType()
        {
            return _context.Equipments
                .GroupBy(e => e.EquipmentType)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }