using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HRMS.Enums;
using HRMS.Service;
using HRMS.ViewModels;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
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
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountService.LoginAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Erreur de connexion");
            return View(model);
        }

        // Création de l'identité et authentification
        var claimsIdentity = new ClaimsIdentity(result.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Session
        HttpContext.Session.SetString("UserRole", result.User!.Role.ToString());
        HttpContext.Session.SetInt32("UserId", result.User.UserId);
        if (result.User.EmployeeId.HasValue)
        {
            HttpContext.Session.SetInt32("EmployeeId", result.User.EmployeeId.Value);
        }

        // Redirection
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
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
        var viewModel = await _accountService.GetRegisterFormViewModelAsync();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "AdminRH")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await _accountService.GetRegisterFormViewModelAsync();
            ViewBag.Model = model;
            return View(viewModel);
        }

        var result = await _accountService.RegisterFromViewModelAsync(model);
        
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Erreur lors de la création de l'utilisateur");
            var viewModel = await _accountService.GetRegisterFormViewModelAsync();
            ViewBag.Model = model;
            return View(viewModel);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = User.Identity?.Name ?? string.Empty;
        var result = await _accountService.ChangePasswordFromViewModelAsync(username, model);
        
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Erreur lors de la modification du mot de passe");
            return View(model);
        }

        TempData["Success"] = "Mot de passe modifié avec succès!";
        return RedirectToAction("Index", "Home");
    }
}