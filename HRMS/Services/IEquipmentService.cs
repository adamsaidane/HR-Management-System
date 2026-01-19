using HRMS.Models;

namespace HRMS.Service;

public interface IEquipmentService
{
    Task<IEnumerable<Equipment>> GetAllEquipmentAsync();
    Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync();
    Task<Equipment?> GetEquipmentByIdAsync(int id);
    Task CreateEquipmentAsync(Equipment equipment);
    Task UpdateEquipmentAsync(Equipment equipment);
    Task DeleteEquipmentAsync(int id);
    Task<IEnumerable<EquipmentAssignment>> GetEmployeeEquipmentAsync(int employeeId);
    Task<IEnumerable<EquipmentAssignment>> GetEquipmentHistoryAsync(int equipmentId);
    Task AssignEquipmentAsync(int equipmentId, int employeeId, string condition, string notes);
    Task ReturnEquipmentAsync(int assignmentId, DateTime returnDate, string condition);
    Task<int> GetTotalEquipmentAsync();
    Task<int> GetAssignedEquipmentCountAsync();
    Task<int> GetAvailableEquipmentCountAsync();
}