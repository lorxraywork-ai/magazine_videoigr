using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.Customers;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.CustomersView)]
public class CustomersController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = dbContext.Customers
            .AsNoTracking()
            .Include(item => item.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.FullName.Contains(search) ||
                (item.Email != null && item.Email.Contains(search)) ||
                (item.Phone != null && item.Phone.Contains(search)));
        }

        return View(new CustomerIndexViewModel
        {
            Search = search,
            Items = await query.OrderBy(item => item.FullName).ToListAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .Include(item => item.User)
            .Include(item => item.Sales)
            .FirstOrDefaultAsync(item => item.CustomerId == id);

        return customer is null ? NotFound() : View(customer);
    }

    [PermissionAuthorize(AppPermissions.CustomersManage)]
    public async Task<IActionResult> Create()
    {
        return View("Upsert", new CustomerFormViewModel
        {
            Users = await LoadClientOptionsAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.CustomersManage)]
    public async Task<IActionResult> Create(CustomerFormViewModel model)
    {
        model.Users = await LoadClientOptionsAsync(model.UserId);
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        dbContext.Customers.Add(new Customer
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Email = model.Email,
            UserId = model.UserId
        });

        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Клиент добавлен.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.CustomersManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await dbContext.Customers.FindAsync(id);
        if (customer is null)
        {
            return NotFound();
        }

        return View("Upsert", new CustomerFormViewModel
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            Phone = customer.Phone,
            Email = customer.Email,
            UserId = customer.UserId,
            Users = await LoadClientOptionsAsync(customer.UserId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.CustomersManage)]
    public async Task<IActionResult> Edit(CustomerFormViewModel model)
    {
        model.Users = await LoadClientOptionsAsync(model.UserId);
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var customer = await dbContext.Customers.FindAsync(model.CustomerId);
        if (customer is null)
        {
            return NotFound();
        }

        customer.FullName = model.FullName;
        customer.Phone = model.Phone;
        customer.Email = model.Email;
        customer.UserId = model.UserId;
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Данные клиента обновлены.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.CustomersManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .Include(item => item.Sales)
            .FirstOrDefaultAsync(item => item.CustomerId == id);

        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.CustomersManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await dbContext.Customers
            .Include(item => item.Sales)
            .FirstOrDefaultAsync(item => item.CustomerId == id);

        if (customer is null)
        {
            return NotFound();
        }

        if (customer.Sales.Count > 0)
        {
            this.SetToastError("Нельзя удалить клиента, у которого уже есть продажи.");
            return RedirectToAction(nameof(Index));
        }

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Клиент удален.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(string? search)
    {
        var query = dbContext.Customers.AsNoTracking().Include(item => item.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.FullName.Contains(search) ||
                (item.Email != null && item.Email.Contains(search)) ||
                (item.Phone != null && item.Phone.Contains(search)));
        }

        var customers = await query.OrderBy(item => item.FullName).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Клиенты",
            ["ID", "ФИО", "Телефон", "Email", "Логин"],
            customers.Select(item => (IReadOnlyList<object?>)[item.CustomerId, item.FullName, item.Phone, item.Email, item.User?.Login]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "customers.xlsx");
    }

    private async Task<List<SelectOptionViewModel>> LoadClientOptionsAsync(int? selectedUserId = null)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .Where(item =>
                item.Role!.RoleName == AppRoles.Client &&
                (item.CustomerProfile == null || item.UserId == selectedUserId))
            .OrderBy(item => item.Login)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.UserId,
                Text = $"{item.Login} ({item.FullName})"
            })
            .ToListAsync();
    }
}
