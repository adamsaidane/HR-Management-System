using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HRMS.Enums;

public class AccountController : Controller
{
    private readonly HRMS.Service.IAccountService _accountService;

    public AccountController(HRMS.Service.IAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET: Login
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST: Account/Login
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Nom d'utilisateur et mot de passe requis");
            return View();
        }

        var user = await _accountService.AuthenticateAsync(username, password);

        if (user != null)
        {

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
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
    public async Task<IActionResult> Register()
    {
        ViewBag.Employees = (await _accountService.GetEmployeesWithoutUserAsync()).ToList();
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Register(string username, string password, string email, int? employeeId, UserRole role)
    {
        var result = await _accountService.RegisterAsync(username, password, email, employeeId, role);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Erreur lors de la création de l'utilisateur");
            ViewBag.Employees = (await _accountService.GetEmployeesWithoutUserAsync()).ToList();
            return View();
        }

        TempData["Success"] = "Utilisateur créé avec succès!";
        return RedirectToAction("Index", "Home");
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

        var username = User.Identity?.Name ?? string.Empty;
        var result = await _accountService.ChangePasswordAsync(username, currentPassword, newPassword);
        if (result.Success)
        {
            TempData["Success"] = "Mot de passe modifié avec succès!";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", result.Error ?? "Erreur lors de la modification du mot de passe");
        return View();
    }

}