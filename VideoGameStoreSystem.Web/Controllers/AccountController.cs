using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Account;

namespace VideoGameStoreSystem.Web.Controllers;

public class AccountController(
    ApplicationDbContext dbContext,
    IPasswordHashingService passwordHashingService,
    IClaimsIdentityFactory claimsIdentityFactory) : Controller
{
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await dbContext.Users
            .Include(item => item.Role)
            .FirstOrDefaultAsync(item => item.Login == model.Login);

        if (user is null || !passwordHashingService.VerifyPassword(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Учетная запись пользователя отключена.");
            return View(model);
        }

        var principal = await claimsIdentityFactory.CreateAsync(user);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(14)
                    : DateTimeOffset.UtcNow.AddHours(8)
            });

        this.SetToastSuccess("Вход выполнен успешно.");

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await dbContext.Users.AnyAsync(item => item.Login == model.Login))
        {
            ModelState.AddModelError(nameof(model.Login), "Пользователь с таким логином уже существует.");
        }

        if (await dbContext.Users.AnyAsync(item => item.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Пользователь с таким email уже существует.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var clientRoleId = await dbContext.Roles
            .Where(item => item.RoleName == AppRoles.Client)
            .Select(item => item.RoleId)
            .FirstAsync();

        var user = new AppUser
        {
            Login = model.Login,
            FullName = model.FullName,
            Email = model.Email,
            RoleId = clientRoleId,
            PasswordHash = passwordHashingService.HashPassword(model.Password),
            IsActive = true
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        dbContext.Customers.Add(new Customer
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            UserId = user.UserId
        });
        await dbContext.SaveChangesAsync();

        user.Role = await dbContext.Roles.FirstAsync(item => item.RoleId == clientRoleId);
        var principal = await claimsIdentityFactory.CreateAsync(user);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        this.SetToastSuccess("Регистрация выполнена. Добро пожаловать в систему.");
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        this.SetToastInfo("Вы вышли из системы.");
        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
