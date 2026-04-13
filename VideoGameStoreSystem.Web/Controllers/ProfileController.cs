using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Profile;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
public class ProfileController(ApplicationDbContext dbContext) : Controller
{
    [PermissionAuthorize(AppPermissions.ProfileView)]
    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = await dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .Include(item => item.CustomerProfile)
            .Where(item => item.UserId == userId.Value)
            .Select(item => new ProfileViewModel
            {
                Login = item.Login,
                FullName = item.FullName,
                Email = item.Email,
                RoleName = item.Role!.RoleName,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                CustomerProfile = item.CustomerProfile
            })
            .FirstOrDefaultAsync();

        return model is null ? NotFound() : View(model);
    }

    [PermissionAuthorize(AppPermissions.PurchaseHistoryView)]
    public async Task<IActionResult> PurchaseHistory()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var sales = await dbContext.Sales
            .AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.SellerUser)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .Where(item => item.Customer != null && item.Customer.UserId == userId.Value)
            .OrderByDescending(item => item.SaleDate)
            .ToListAsync();

        return View(new PurchaseHistoryViewModel { Sales = sales });
    }
}
