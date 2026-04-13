using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.Supplies;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.SuppliesView)]
public class SuppliesController(
    ApplicationDbContext dbContext,
    IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index(int? supplierId, DateTime? dateFrom, DateTime? dateTo, decimal? minAmount, decimal? maxAmount)
    {
        var query = dbContext.Supplies
            .AsNoTracking()
            .Include(item => item.Supplier)
            .Include(item => item.Items)
            .AsQueryable();

        if (supplierId.HasValue)
        {
            query = query.Where(item => item.SupplierId == supplierId.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.SupplyDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.SupplyDate < dateTo.Value.Date.AddDays(1));
        }

        if (minAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount <= maxAmount.Value);
        }

        return View(new SupplyIndexViewModel
        {
            SupplierId = supplierId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            MinAmount = minAmount,
            MaxAmount = maxAmount,
            Items = await query.OrderByDescending(item => item.SupplyDate).ToListAsync(),
            Suppliers = await LoadSupplierOptionsAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var supply = await dbContext.Supplies
            .AsNoTracking()
            .Include(item => item.Supplier)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.SupplyId == id);

        return supply is null ? NotFound() : View(supply);
    }

    [PermissionAuthorize(AppPermissions.SuppliesManage)]
    public async Task<IActionResult> Create()
    {
        return View("Upsert", await BuildViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SuppliesManage)]
    public async Task<IActionResult> Create(SupplyEditViewModel model)
    {
        model.Items = NormalizeItems(model.Items);
        await FillLookupsAsync(model);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну позицию поступления.");
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var supply = new Supply
        {
            SupplyDate = model.SupplyDate,
            SupplierId = model.SupplierId,
            TotalAmount = model.Items.Sum(item => item.Quantity * item.UnitCost),
            Items = model.Items.Select(item => new SupplyItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost
            }).ToList()
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.ApplySupplyAsync(model.Items);
            dbContext.Supplies.Add(supply);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Поступление создано.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Upsert", model);
        }
    }

    [PermissionAuthorize(AppPermissions.SuppliesManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var supply = await dbContext.Supplies
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.SupplyId == id);

        return supply is null ? NotFound() : View("Upsert", await BuildViewModelAsync(supply));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SuppliesManage)]
    public async Task<IActionResult> Edit(SupplyEditViewModel model)
    {
        model.Items = NormalizeItems(model.Items);
        await FillLookupsAsync(model);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну позицию поступления.");
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var supply = await dbContext.Supplies
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.SupplyId == model.SupplyId);

        if (supply is null)
        {
            return NotFound();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.RevertSupplyAsync(supply.Items);
            dbContext.SupplyItems.RemoveRange(supply.Items);

            supply.SupplyDate = model.SupplyDate;
            supply.SupplierId = model.SupplierId;
            supply.TotalAmount = model.Items.Sum(item => item.Quantity * item.UnitCost);
            supply.Items = model.Items.Select(item => new SupplyItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost
            }).ToList();

            await inventoryService.ApplySupplyAsync(model.Items);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Поступление обновлено.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            dbContext.ChangeTracker.Clear();
            ModelState.AddModelError(string.Empty, exception.Message);
            await FillLookupsAsync(model);
            return View("Upsert", model);
        }
    }

    [PermissionAuthorize(AppPermissions.SuppliesManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var supply = await dbContext.Supplies
            .AsNoTracking()
            .Include(item => item.Supplier)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.SupplyId == id);

        return supply is null ? NotFound() : View(supply);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SuppliesManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supply = await dbContext.Supplies
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.SupplyId == id);

        if (supply is null)
        {
            return NotFound();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.RevertSupplyAsync(supply.Items);
            dbContext.Supplies.Remove(supply);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Поступление удалено.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            this.SetToastError(exception.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(int? supplierId, DateTime? dateFrom, DateTime? dateTo, decimal? minAmount, decimal? maxAmount)
    {
        var query = dbContext.Supplies.AsNoTracking().Include(item => item.Supplier).AsQueryable();
        if (supplierId.HasValue)
        {
            query = query.Where(item => item.SupplierId == supplierId.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.SupplyDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.SupplyDate < dateTo.Value.Date.AddDays(1));
        }

        if (minAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount <= maxAmount.Value);
        }

        var supplies = await query.OrderByDescending(item => item.SupplyDate).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Поступления товаров",
            ["ID", "Дата", "Поставщик", "Сумма"],
            supplies.Select(item => (IReadOnlyList<object?>)[item.SupplyId, item.SupplyDate, item.Supplier?.SupplierName, item.TotalAmount]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "supplies.xlsx");
    }

    private async Task<SupplyEditViewModel> BuildViewModelAsync(Supply? supply = null)
    {
        var model = new SupplyEditViewModel
        {
            SupplyId = supply?.SupplyId ?? 0,
            SupplyDate = supply?.SupplyDate ?? DateTime.Today,
            SupplierId = supply?.SupplierId ?? 0,
            Items = supply?.Items.Select(item => new SupplyItemInputViewModel
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost
            }).ToList() ?? [new()]
        };

        await FillLookupsAsync(model);
        return model;
    }

    private async Task FillLookupsAsync(SupplyEditViewModel model)
    {
        model.Suppliers = await LoadSupplierOptionsAsync();
        model.Products = await LoadProductOptionsAsync();
    }

    private async Task<List<SelectOptionViewModel>> LoadSupplierOptionsAsync()
    {
        return await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(item => item.SupplierName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.SupplierId,
                Text = item.SupplierName
            })
            .ToListAsync();
    }

    private async Task<List<SelectOptionViewModel>> LoadProductOptionsAsync()
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(item => item.ProductName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.ProductId,
                Text = $"{item.ProductName} ({item.Sku})"
            })
            .ToListAsync();
    }

    private static List<SupplyItemInputViewModel> NormalizeItems(IEnumerable<SupplyItemInputViewModel> items)
    {
        return items
            .Where(item => item.ProductId > 0 && item.Quantity > 0 && item.UnitCost > 0)
            .ToList();
    }
}
