using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Suppliers;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.SuppliersView)]
public class SuppliersController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = dbContext.Suppliers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.SupplierName.Contains(search) ||
                (item.Email != null && item.Email.Contains(search)) ||
                (item.Phone != null && item.Phone.Contains(search)));
        }

        return View(new SupplierIndexViewModel
        {
            Search = search,
            Items = await query.OrderBy(item => item.SupplierName).ToListAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var supplier = await dbContext.Suppliers
            .AsNoTracking()
            .Include(item => item.Supplies)
            .FirstOrDefaultAsync(item => item.SupplierId == id);

        return supplier is null ? NotFound() : View(supplier);
    }

    [PermissionAuthorize(AppPermissions.SuppliersManage)]
    public IActionResult Create()
    {
        return View("Upsert", new SupplierFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SuppliersManage)]
    public async Task<IActionResult> Create(SupplierFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        dbContext.Suppliers.Add(new Supplier
        {
            SupplierName = model.SupplierName,
            Phone = model.Phone,
            Email = model.Email
        });

        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Поставщик добавлен.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.SuppliersManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await dbContext.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return NotFound();
        }

        return View("Upsert", new SupplierFormViewModel
        {
            SupplierId = supplier.SupplierId,
            SupplierName = supplier.SupplierName,
            Phone = supplier.Phone,
            Email = supplier.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SuppliersManage)]
    public async Task<IActionResult> Edit(SupplierFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var supplier = await dbContext.Suppliers.FindAsync(model.SupplierId);
        if (supplier is null)
        {
            return NotFound();
        }

        supplier.SupplierName = model.SupplierName;
        supplier.Phone = model.Phone;
        supplier.Email = model.Email;
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Данные поставщика обновлены.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.SuppliersManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await dbContext.Suppliers
            .AsNoTracking()
            .Include(item => item.Supplies)
            .FirstOrDefaultAsync(item => item.SupplierId == id);

        return supplier is null ? NotFound() : View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SuppliersManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supplier = await dbContext.Suppliers
            .Include(item => item.Supplies)
            .FirstOrDefaultAsync(item => item.SupplierId == id);

        if (supplier is null)
        {
            return NotFound();
        }

        if (supplier.Supplies.Count > 0)
        {
            this.SetToastError("Нельзя удалить поставщика, у которого есть поступления.");
            return RedirectToAction(nameof(Index));
        }

        dbContext.Suppliers.Remove(supplier);
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Поставщик удален.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(string? search)
    {
        var query = dbContext.Suppliers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.SupplierName.Contains(search) ||
                (item.Email != null && item.Email.Contains(search)) ||
                (item.Phone != null && item.Phone.Contains(search)));
        }

        var suppliers = await query.OrderBy(item => item.SupplierName).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Поставщики",
            ["ID", "Поставщик", "Телефон", "Email"],
            suppliers.Select(item => (IReadOnlyList<object?>)[item.SupplierId, item.SupplierName, item.Phone, item.Email]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "suppliers.xlsx");
    }
}
