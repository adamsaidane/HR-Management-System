using HRMS.Models;

namespace HRMS.ViewModels;

public class InterviewFormViewModel
{
    public Candidate Candidate { get; set; } = null!;
    public Interview Interview { get; set; } = new();
}