using HRMS.Models;

namespace HRMS.ViewModels;

public class CandidateDetailsViewModel
{
    public Candidate Candidate { get; set; } = null!;
    public List<Interview> Interviews { get; set; } = new();
}