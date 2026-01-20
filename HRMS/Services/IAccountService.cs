using System.Security.Claims;
using HRMS.Enums;
using HRMS.Models;
using HRMS.ViewModels;

namespace HRMS.Service;

public interface IAccountService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<(bool Success, string? Error)> RegisterAsync(string username, string password, string email, int? employeeId, UserRole role);
    Task<(bool Success, string? Error)> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    Task<IEnumerable<Employee>> GetEmployeesWithoutUserAsync();
    Task<LoginResult> LoginAsync(LoginViewModel model);
    Task<RegisterFormViewModel> GetRegisterFormViewModelAsync();
    Task<(bool Success, string? Error)> RegisterFromViewModelAsync(RegisterViewModel model);
    Task<(bool Success, string? Error)> ChangePasswordFromViewModelAsync(string username, ChangePasswordViewModel model);
    List<Claim> CreateUserClaims(User user);
}