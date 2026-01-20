using System.Security.Claims;
using HRMS.Models;

namespace HRMS.ViewModels;

public class LoginResult
{
    public bool Success { get; set; }
    public User? User { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Claim> Claims { get; set; } = new();
}