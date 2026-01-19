using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Service;

public interface IAccountService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<(bool Success, string? Error)> RegisterAsync(string username, string password, string email, int? employeeId, UserRole role);
    Task<(bool Success, string? Error)> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    Task<IEnumerable<Employee>> GetEmployeesWithoutUserAsync();
}