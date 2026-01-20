using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.ViewModels;

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
        
        if (await _unitOfWork.Users.ExistsAsync(u => u.Email == email))
        {
            return (false, "Ce mail existe déjà");
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
    
    public async Task<LoginResult> LoginAsync(LoginViewModel model)
    {
        // Validation des entrées
        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Nom d'utilisateur et mot de passe requis"
            };
        }

        // Authentification
        var user = await AuthenticateAsync(model.Username, model.Password);
        
        if (user == null)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect"
            };
        }

        // Création des claims
        var claims = CreateUserClaims(user);

        return new LoginResult
        {
            Success = true,
            User = user,
            Claims = claims
        };
    }

    public async Task<RegisterFormViewModel> GetRegisterFormViewModelAsync()
    {
        var employees = await GetEmployeesWithoutUserAsync();
        
        return new RegisterFormViewModel
        {
            AvailableEmployees = employees.ToList(),
            AvailableRoles = Enum.GetValues<UserRole>().ToList()
        };
    }

    public async Task<(bool Success, string? Error)> RegisterFromViewModelAsync(RegisterViewModel model)
    {
        // Validation du modèle
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            return (false, "Le nom d'utilisateur est requis");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            return (false, "Le mot de passe est requis");
        }

        if (model.Password != model.ConfirmPassword)
        {
            return (false, "Les mots de passe ne correspondent pas");
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return (false, "L'email est requis");
        }

        // Validation de l'email
        if (!IsValidEmail(model.Email))
        {
            return (false, "Format d'email invalide");
        }

        // Validation de la force du mot de passe
        var passwordValidation = ValidatePasswordStrength(model.Password);
        if (!passwordValidation.IsValid)
        {
            return (false, passwordValidation.ErrorMessage);
        }

        // Vérification si l'employé sélectionné a déjà un compte
        if (model.EmployeeId.HasValue)
        {
            var employeeHasUser = await _unitOfWork.Users.ExistsAsync(u => u.EmployeeId == model.EmployeeId.Value);
            if (employeeHasUser)
            {
                return (false, "Cet employé a déjà un compte utilisateur");
            }
        }

        // Création du compte
        return await RegisterAsync(
            model.Username,
            model.Password,
            model.Email,
            model.EmployeeId,
            model.Role);
    }

    public async Task<(bool Success, string? Error)> ChangePasswordFromViewModelAsync(string username, ChangePasswordViewModel model)
    {
        // Validation du modèle
        if (string.IsNullOrWhiteSpace(model.CurrentPassword))
        {
            return (false, "Le mot de passe actuel est requis");
        }

        if (string.IsNullOrWhiteSpace(model.NewPassword))
        {
            return (false, "Le nouveau mot de passe est requis");
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            return (false, "Les nouveaux mots de passe ne correspondent pas");
        }

        // Validation que le nouveau mot de passe est différent de l'ancien
        if (model.CurrentPassword == model.NewPassword)
        {
            return (false, "Le nouveau mot de passe doit être différent de l'ancien");
        }

        // Validation de la force du mot de passe
        var passwordValidation = ValidatePasswordStrength(model.NewPassword);
        if (!passwordValidation.IsValid)
        {
            return (false, passwordValidation.ErrorMessage);
        }

        return await ChangePasswordAsync(username, model.CurrentPassword, model.NewPassword);
    }

    public List<Claim> CreateUserClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.EmployeeId.HasValue)
        {
            claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
            
            if (user.Employee != null)
            {
                claims.Add(new Claim("FullName", $"{user.Employee.FirstName} {user.Employee.LastName}"));
            }
        }

        return claims;
    }

    // ==================== MÉTHODES PRIVÉES DE VALIDATION ====================

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static (bool IsValid, string? ErrorMessage) ValidatePasswordStrength(string password)
    {
        if (password.Length < 6)
        {
            return (false, "Le mot de passe doit contenir au moins 6 caractères");
        }

        if (password.Length > 100)
        {
            return (false, "Le mot de passe ne peut pas dépasser 100 caractères");
        }
        return (true, null);
    }
}