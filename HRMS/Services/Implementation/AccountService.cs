using System.Security.Cryptography;
using System.Text;
using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;

namespace HRMS.Service;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesWithoutUserAsync()
    {
        var list = await _unitOfWork.Employees.FindAsync(e => e.User == null);
        return list;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var passwordHash = HashPassword(password);
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u =>
            u.Username == username &&
            u.PasswordHash == passwordHash &&
            u.IsActive);

        if (user != null && user.EmployeeId.HasValue && user.Employee == null)
        {
            user.Employee = await _unitOfWork.Employees.GetByIdAsync(user.EmployeeId.Value);
        }

        return user;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string username, string password, string email, int? employeeId, UserRole role)
    {
        if (await _unitOfWork.Users.ExistsAsync(u => u.Username == username))
        {
            return (false, "Ce nom d'utilisateur existe déjà");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            Email = email,
            EmployeeId = employeeId,
            Role = role,
            IsActive = true,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(string username, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return (false, "Utilisateur introuvable");
        }

        if (user.PasswordHash != HashPassword(currentPassword))
        {
            return (false, "Mot de passe actuel incorrect");
        }

        user.PasswordHash = HashPassword(newPassword);
        user.ModifiedDate = DateTime.Now;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}