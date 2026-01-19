using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Repositories;

public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync();
    Task<IEnumerable<Equipment>> GetByStatusAsync(EquipmentStatus status);
    Task<Equipment?> GetByIdWithHistoryAsync(int id);
}
