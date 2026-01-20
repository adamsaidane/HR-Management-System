using HRMS.Models;

namespace HRMS.ViewModels;

public class EquipmentIndexViewModel
{
    public List<Equipment> Equipment { get; set; } = new();
    public List<string> EquipmentTypes { get; set; } = new();
}