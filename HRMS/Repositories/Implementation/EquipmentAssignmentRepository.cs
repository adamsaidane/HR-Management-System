using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class EquipmentAssignmentRepository : Repository<EquipmentAssignment>, IEquipmentAssignmentRepository
{
    public EquipmentAssignmentRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<EquipmentAssignment>> GetEmployeeEquipmentAsync(int employeeId)
    {
        return await _dbSet
            .Include(ea => ea.Equipment)
            .Where(ea => ea.EmployeeId == employeeId)
            .OrderByDescending(ea => ea.AssignmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EquipmentAssignment>> GetEquipmentHistoryAsync(int equipmentId)
    {
        return await _dbSet
            .Include(ea => ea.Employee)
            .Where(ea => ea.EquipmentId == equipmentId)
            .OrderByDescending(ea => ea.AssignmentDate)
            .ToListAsync();
    }
}
