using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.Users;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.UsersView)]
public class UsersController(
    ApplicationDbContext dbContext,
    IPasswordHashingService passwordHashingService) : Controller
{
    public async Task<IActionResult> Index(string? search, int? roleId, bool? isActive)
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.Login.Contains(search) ||
                item.FullName.Contains(search) ||
                item.Email.Contains(search));
        }

        if (roleId.HasValue)
        {
            query = query.Where(item => item.RoleId == roleId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(item => item.IsActive == isActive.Value);
        }

        return View(new UserIndexViewModel
        {
            Search = search,
            RoleId = roleId,
            IsActive = isActive,
            Items = await query.OrderBy(item => item.Login).ToListAsync(),
            Roles = await LoadRoleOptionsAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .Include(item => item.CustomerProfile)
            .FirstOrDefaultAsync(item => item.UserId == id);

        return user is null ? NotFound() : View(user);
    }

    [PermissionAuthorize(AppPermissions.UsersManage)]
    public async Task<IActionResult> Create()
    {
        return View("Upsert", new UserFormViewModel
        {
            IsActive = true,
            Roles = await LoadRoleOptionsAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.UsersManage)]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        model.Roles = await LoadRoleOptionsAsync();
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Для нового пользователя нужно задать пароль.");
        }

        if (await dbContext.Users.AnyAsync(item => item.Login == model.Login))
        {
            ModelState.AddModelError(nameof(model.Login), "Пользователь с таким логином уже существует.");
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var user = new AppUser
        {
            Login = model.Login,
            FullName = model.FullName,
            Email = model.Email,
            RoleId = model.RoleId,
            IsActive = model.IsActive,
            PasswordHash = passwordHashingService.HashPassword(model.Password!)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        await SynchronizeCustomerForClientRoleAsync(user);

        this.SetToastSuccess("Пользователь добавлен.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.UsersManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return View("Upsert", new UserFormViewModel
        {
            UserId = user.UserId,
            Login = user.Login,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            Roles = await LoadRoleOptionsAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.UsersManage)]
    public async Task<IActionResult> Edit(UserFormViewModel model)
    {
        model.Roles = await LoadRoleOptionsAsync();
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var user = await dbContext.Users.FindAsync(model.UserId);
        if (user is null)
        {
            return NotFound();
        }

        if (await dbContext.Users.AnyAsync(item =>
                item.Login == model.Login &&
                item.UserId != model.UserId))
        {
            ModelState.AddModelError(nameof(model.Login), "Пользователь с таким логином уже существует.");
            return View("Upsert", model);
        }

        user.Login = model.Login;
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.RoleId = model.RoleId;
        user.IsActive = model.IsActive;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            user.PasswordHash = passwordHashingService.HashPassword(model.Password);
        }

        await dbContext.SaveChangesAsync();
        await SynchronizeCustomerForClientRoleAsync(user);

        this.SetToastSuccess("Данные пользователя обновлены.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.UsersManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .FirstOrDefaultAsync(item => item.UserId == id);

        return user is null ? NotFound() : View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.UsersManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (User.GetUserId() == id)
        {
            this.SetToastError("Нельзя удалить текущую учетную запись.");
            return RedirectToAction(nameof(Index));
        }

        var user = await dbContext.Users
            .Include(item => item.CustomerProfile)
            .Include(item => item.SalesAsSeller)
            .Include(item => item.WriteOffsCreated)
            .Include(item => item.Changes)
            .FirstOrDefaultAsync(item => item.UserId == id);

        if (user is null)
        {
            return NotFound();
        }

        if (user.SalesAsSeller.Count > 0 || user.WriteOffsCreated.Count > 0 || user.Changes.Count > 0)
        {
            this.SetToastError("Нельзя удалить пользователя, который участвует в документах или журнале изменений.");
            return RedirectToAction(nameof(Index));
        }

        if (user.CustomerProfile is not null)
        {
            dbContext.Customers.Remove(user.CustomerProfile);
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Пользователь удален.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(string? search, int? roleId, bool? isActive)
    {
        var query = dbContext.Users.AsNoTracking().Include(item => item.Role).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.Login.Contains(search) ||
                item.FullName.Contains(search) ||
                item.Email.Contains(search));
        }

        if (roleId.HasValue)
        {
            query = query.Where(item => item.RoleId == roleId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(item => item.IsActive == isActive.Value);
        }

        var users = await query.OrderBy(item => item.Login).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Пользователи системы",
            ["ID", "Логин", "ФИО", "Email", "Роль", "Активен", "Создан"],
            users.Select(item => (IReadOnlyList<object?>)
            [
                item.UserId,
                item.Login,
                item.FullName,
                item.Email,
                item.Role?.RoleName,
                item.IsActive ? "Да" : "Нет",
                item.CreatedAt
            ]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
    }

    private async Task<List<SelectOptionViewModel>> LoadRoleOptionsAsync()
    {
        return await dbContext.Roles
            .AsNoTracking()
            .OrderBy(item => item.RoleName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.RoleId,
                Text = item.RoleName
            })
            .ToListAsync();
    }

    private async Task SynchronizeCustomerForClientRoleAsync(AppUser user)
    {
        var roleName = await dbContext.Roles
            .Where(item => item.RoleId == user.RoleId)
            .Select(item => item.RoleName)
            .FirstAsync();

        var customer = await dbContext.Customers.FirstOrDefaultAsync(item => item.UserId == user.UserId);
        if (roleName == AppRoles.Client)
        {
            if (customer is null)
            {
                dbContext.Customers.Add(new Customer
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserId = user.UserId
                });
            }
            else
            {
                customer.FullName = user.FullName;
                customer.Email = user.Email;
            }
        }
        else if (customer is not null)
        {
            customer.UserId = null;
        }

        await dbContext.SaveChangesAsync();
    }
}
