using HRMS.Models;

namespace HRMS.ViewModels;

public class JobOfferFormViewModel
{
    public JobOffer JobOffer { get; set; }
    public List<Department> Departments { get; set; } = new();
    public List<Position> Positions { get; set; } = new();
}