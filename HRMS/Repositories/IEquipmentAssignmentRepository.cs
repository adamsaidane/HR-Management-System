using HRMS.Models;

namespace HRMS.Repositories;

public interface IEquipmentAssignmentRepository : IRepository<EquipmentAssignment>
{
    Task<IEnumerable<EquipmentAssignment>> GetEmployeeEquipmentAsync(int employeeId);
    Task<IEnumerable<EquipmentAssignment>> GetEquipmentHistoryAsync(int equipmentId);
}
