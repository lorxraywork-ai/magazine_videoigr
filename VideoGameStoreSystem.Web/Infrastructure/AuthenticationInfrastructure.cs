using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Models;

namespace VideoGameStoreSystem.Web.Infrastructure;

public interface IPasswordHashingService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}

public class PasswordHashingService : IPasswordHashingService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4 || !parts[0].Equals("PBKDF2", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[2]);
        var expected = Convert.FromBase64String(parts[3]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public interface ICurrentUserService
{
    int? UserId { get; }
}

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var userId) ? userId : null;
        }
    }
}

public class NullCurrentUserService : ICurrentUserService
{
    public int? UserId => null;
}

public interface IClaimsIdentityFactory
{
    Task<ClaimsPrincipal> CreateAsync(AppUser user);
}

public class ClaimsIdentityFactory(ApplicationDbContext dbContext) : IClaimsIdentityFactory
{
    public async Task<ClaimsPrincipal> CreateAsync(AppUser user)
    {
        var roleName = user.Role?.RoleName;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            roleName = await dbContext.Roles
                .Where(role => role.RoleId == user.RoleId)
                .Select(role => role.RoleName)
                .FirstAsync();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.GivenName, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, roleName)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);

        return new ClaimsPrincipal(identity);
    }
}

public class AppCookieAuthenticationEvents(
    IServiceScopeFactory scopeFactory,
    ILogger<AppCookieAuthenticationEvents> logger) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var claimsFactory = scope.ServiceProvider.GetRequiredService<IClaimsIdentityFactory>();
        var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            await RejectPrincipalAsync(context);
            return;
        }

        var user = await dbContext.Users
            .Include(item => item.Role)
            .FirstOrDefaultAsync(item => item.UserId == userId);

        if (user is null || !user.IsActive)
        {
            await RejectPrincipalAsync(context);
            return;
        }

        var currentName = context.Principal?.FindFirstValue(ClaimTypes.GivenName);
        var currentRole = context.Principal?.FindFirstValue(ClaimTypes.Role);
        var currentLogin = context.Principal?.Identity?.Name;

        if (currentName == user.FullName && currentRole == user.Role?.RoleName && currentLogin == user.Login)
        {
            return;
        }

        logger.LogInformation("Обновлены claims пользователя {Login}", user.Login);

        var principal = await claimsFactory.CreateAsync(user);
        context.ReplacePrincipal(principal);
        context.ShouldRenew = true;
    }

    private static async Task RejectPrincipalAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}

public static class PrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
