using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories.Implementation;

public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(HRMSDbContext context) : base(context) { }

    public async Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync()
    {
        return await _dbSet
            .Where(e => e.Status == EquipmentStatus.Disponible)
            .OrderBy(e => e.EquipmentType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetByStatusAsync(EquipmentStatus status)
    {
        return await _dbSet
            .Where(e => e.Status == status)
            .OrderBy(e => e.EquipmentType)
            .ToListAsync();
    }

    public async Task<Equipment?> GetByIdWithHistoryAsync(int id)
    {
        return await _dbSet
            .Include(e => e.EquipmentAssignments)
            .ThenInclude(ea => ea.Employee)
            .FirstOrDefaultAsync(e => e.EquipmentId == id);
    }
}