using HRMS.Models;

namespace HRMS.ViewModels;

public class EquipmentPaginatedIndexViewModel
{
    public PaginatedList<Equipment> Equipment { get; set; }
    public List<string> EquipmentTypes { get; set; } = new();
}