using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.WriteOffs;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.WriteOffsView)]
public class WriteOffsController(
    ApplicationDbContext dbContext,
    IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo, string? reason)
    {
        var query = dbContext.WriteOffs
            .AsNoTracking()
            .Include(item => item.CreatedByUser)
            .Include(item => item.Items)
            .AsQueryable();

        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.WriteOffDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.WriteOffDate < dateTo.Value.Date.AddDays(1));
        }

        if (!string.IsNullOrWhiteSpace(reason))
        {
            query = query.Where(item => item.Reason.Contains(reason));
        }

        return View(new WriteOffIndexViewModel
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Reason = reason,
            Items = await query.OrderByDescending(item => item.WriteOffDate).ToListAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var writeOff = await dbContext.WriteOffs
            .AsNoTracking()
            .Include(item => item.CreatedByUser)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.WriteOffId == id);

        return writeOff is null ? NotFound() : View(writeOff);
    }

    [PermissionAuthorize(AppPermissions.WriteOffsManage)]
    public async Task<IActionResult> Create()
    {
        return View("Upsert", await BuildViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.WriteOffsManage)]
    public async Task<IActionResult> Create(WriteOffEditViewModel model)
    {
        RemoveIgnoredItemValidationErrors(model.Items);
        model.Items = NormalizeItems(model.Items);
        await FillLookupsAsync(model);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну позицию списания.");
        }

        var errors = await inventoryService.ValidateWriteOffAsync(model.Items);
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var writeOff = new WriteOff
        {
            WriteOffDate = model.WriteOffDate,
            Reason = model.Reason,
            CreatedByUserId = User.GetUserId() ?? 0,
            Items = model.Items.Select(item => new WriteOffItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.ApplyWriteOffAsync(model.Items);
            dbContext.WriteOffs.Add(writeOff);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Списание создано.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Upsert", model);
        }
    }

    [PermissionAuthorize(AppPermissions.WriteOffsManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var writeOff = await dbContext.WriteOffs
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.WriteOffId == id);

        return writeOff is null ? NotFound() : View("Upsert", await BuildViewModelAsync(writeOff));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.WriteOffsManage)]
    public async Task<IActionResult> Edit(WriteOffEditViewModel model)
    {
        RemoveIgnoredItemValidationErrors(model.Items);
        model.Items = NormalizeItems(model.Items);
        await FillLookupsAsync(model);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну позицию списания.");
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var writeOff = await dbContext.WriteOffs
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.WriteOffId == model.WriteOffId);

        if (writeOff is null)
        {
            return NotFound();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.RevertWriteOffAsync(writeOff.Items);
            dbContext.WriteOffItems.RemoveRange(writeOff.Items);

            var errors = await inventoryService.ValidateWriteOffAsync(model.Items);
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (!ModelState.IsValid)
            {
                await transaction.RollbackAsync();
                dbContext.ChangeTracker.Clear();
                await FillLookupsAsync(model);
                return View("Upsert", model);
            }

            writeOff.WriteOffDate = model.WriteOffDate;
            writeOff.Reason = model.Reason;
            writeOff.Items = model.Items.Select(item => new WriteOffItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            await inventoryService.ApplyWriteOffAsync(model.Items);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Списание обновлено.");
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

    [PermissionAuthorize(AppPermissions.WriteOffsManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var writeOff = await dbContext.WriteOffs
            .AsNoTracking()
            .Include(item => item.CreatedByUser)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.WriteOffId == id);

        return writeOff is null ? NotFound() : View(writeOff);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.WriteOffsManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var writeOff = await dbContext.WriteOffs
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.WriteOffId == id);

        if (writeOff is null)
        {
            return NotFound();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.RevertWriteOffAsync(writeOff.Items);
            dbContext.WriteOffs.Remove(writeOff);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Списание удалено.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            this.SetToastError(exception.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(DateTime? dateFrom, DateTime? dateTo, string? reason)
    {
        var query = dbContext.WriteOffs.AsNoTracking().Include(item => item.CreatedByUser).AsQueryable();
        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.WriteOffDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.WriteOffDate < dateTo.Value.Date.AddDays(1));
        }

        if (!string.IsNullOrWhiteSpace(reason))
        {
            query = query.Where(item => item.Reason.Contains(reason));
        }

        var writeOffs = await query.OrderByDescending(item => item.WriteOffDate).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Списания товаров",
            ["ID", "Дата", "Причина", "Создал"],
            writeOffs.Select(item => (IReadOnlyList<object?>)[item.WriteOffId, item.WriteOffDate, item.Reason, item.CreatedByUser?.Login]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "write-offs.xlsx");
    }

    private async Task<WriteOffEditViewModel> BuildViewModelAsync(WriteOff? writeOff = null)
    {
        var model = new WriteOffEditViewModel
        {
            WriteOffId = writeOff?.WriteOffId ?? 0,
            WriteOffDate = writeOff?.WriteOffDate ?? DateTime.Today,
            Reason = writeOff?.Reason ?? string.Empty,
            Items = writeOff?.Items.Select(item => new WriteOffItemInputViewModel
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList() ?? [new()]
        };

        await FillLookupsAsync(model);
        return model;
    }

    private async Task FillLookupsAsync(WriteOffEditViewModel model)
    {
        model.Products = await dbContext.Products
            .AsNoTracking()
            .OrderBy(item => item.ProductName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.ProductId,
                Text = $"{item.ProductName} (остаток: {item.StockQuantity})"
            })
            .ToListAsync();
    }

    private static List<WriteOffItemInputViewModel> NormalizeItems(IEnumerable<WriteOffItemInputViewModel> items)
    {
        return items
            .Where(item => item.ProductId > 0 && item.Quantity > 0)
            .ToList();
    }

    private void RemoveIgnoredItemValidationErrors(IReadOnlyList<WriteOffItemInputViewModel> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (!IsEmptyItemRow(items[i]))
            {
                continue;
            }

            ModelState.Remove($"Items[{i}].ProductId");
            ModelState.Remove($"Items[{i}].Quantity");
        }
    }

    private static bool IsEmptyItemRow(WriteOffItemInputViewModel item)
    {
        return item.ProductId <= 0 && item.Quantity <= 0;
    }
}
