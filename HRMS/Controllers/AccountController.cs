using HRMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HRMS.Enums;
using Microsoft.EntityFrameworkCore;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly HRMSDbContext _context;
    public AccountController(HRMSDbContext context) {
        _context = context;
    }

    // GET: Login
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Nom d'utilisateur et mot de passe requis");
            return View();
        }

        var passwordHash = HashPassword(password);
        var user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => 
                u.Username == username && 
                u.PasswordHash == passwordHash && 
                u.IsActive);

        if (user != null) 
        { 
            // Créer les claims
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, user.Username), 
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), 
                new Claim(ClaimTypes.Email, user.Email), 
                new Claim(ClaimTypes.Role, user.Role.ToString()), 
                new Claim("EmployeeId", user.EmployeeId?.ToString() ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
            var authProperties = new AuthenticationProperties 
            { 
                IsPersistent = true, 
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            // Stocker des informations dans la session
            HttpContext.Session.SetString("UserRole", user.Role.ToString()); 
            HttpContext.Session.SetInt32("UserId", user.UserId); 
            if (user.EmployeeId.HasValue) 
            { 
                HttpContext.Session.SetInt32("EmployeeId", user.EmployeeId.Value);
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) 
            { 
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect");
        return View();
    }

    // GET: Account/Logout
    public async Task<IActionResult> Logout()
    { await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear(); 
        return RedirectToAction("Login");
    }

    // GET: Account/AccessDenied
    [AllowAnonymous]
    public IActionResult AccessDenied()
    { 
        return View();
    }

    // GET: Account/Register
    [Authorize(Roles = "AdminRH")]
    public IActionResult Register()
    {
        ViewBag.Employees = _context.Employees
            .Where(e => e.User == null)
            .ToList();
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Register(string username, string password, string email, int? employeeId, UserRole role)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError("", "Ce nom d'utilisateur existe déjà");
            ViewBag.Employees = await _context.Employees
                .Where(e => e.User == null)
                .ToListAsync();
            return View();
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

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Utilisateur créé avec succès!"; 
        return RedirectToAction("Login");
    }

    // GET: Account/ChangePassword
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST: ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Les nouveaux mots de passe ne correspondent pas");
            return View();
        }

        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username); 
            
        if (user != null && user.PasswordHash == HashPassword(currentPassword))
        { 
            user.PasswordHash = HashPassword(newPassword);
            user.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Mot de passe modifié avec succès!";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Mot de passe actuel incorrect");
        return View();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}
