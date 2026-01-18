using HRMS.Models;

namespace HRMS.Service;

public interface IEquipmentService
{
    // Equipment CRUD
    IEnumerable<Equipment> GetAllEquipment();
    IEnumerable<Equipment> GetAvailableEquipment();
    Equipment GetEquipmentById(int id);
    void CreateEquipment(Equipment equipment);
    void UpdateEquipment(Equipment equipment);
    void DeleteEquipment(int id);

    // Assignments
    IEnumerable<EquipmentAssignment> GetEmployeeEquipment(int employeeId);
    IEnumerable<EquipmentAssignment> GetEquipmentHistory(int equipmentId);
    void AssignEquipment(int equipmentId, int employeeId, string condition, string notes);
    void ReturnEquipment(int assignmentId, DateTime returnDate, string condition);

    // Statistics
    int GetTotalEquipment();
    int GetAssignedEquipmentCount();
    int GetAvailableEquipmentCount();
    Dictionary<string, int> GetEquipmentByType();
}