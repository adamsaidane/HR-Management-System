using HRMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly HRMSDbContext _context;

    public AccountController(HRMSDbContext context)
    {
        _context = context;
    }

    // GET: Login
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST: Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string returnUrl)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Nom d'utilisateur et mot de passe requis");
            return View();
        }

        var passwordHash = HashPassword(password);
        var user = _context.Users.FirstOrDefault(u =>
            u.Username == username &&
            u.PasswordHash == passwordHash &&
            u.IsActive);

        if (user != null)
        {
            // Créer les claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("EmployeeId", user.EmployeeId?.ToString() ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect");
        return View();
    }

    // GET: Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    // GET: ChangePassword
    [Authorize]
    public IActionResult ChangePassword() => View();

    // POST: ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Les nouveaux mots de passe ne correspondent pas");
            return View();
        }

        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);

        if (user != null && user.PasswordHash == HashPassword(currentPassword))
        {
            user.PasswordHash = HashPassword(newPassword);
            user.ModifiedDate = DateTime.Now;
            _context.SaveChanges();

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
