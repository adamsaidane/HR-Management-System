using HRMS.Enums;
using HRMS.Models;

namespace HRMS.ViewModels;

public class RegisterFormViewModel
{
    public List<Employee> AvailableEmployees { get; set; } = new();
    public List<UserRole> AvailableRoles { get; set; } = new();
}