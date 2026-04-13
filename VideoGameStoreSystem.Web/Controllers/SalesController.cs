using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.Sales;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.SalesView)]
public class SalesController(
    ApplicationDbContext dbContext,
    IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo, int? sellerUserId, decimal? minAmount, decimal? maxAmount)
    {
        var query = dbContext.Sales
            .AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.SellerUser)
            .Include(item => item.Items)
            .AsQueryable();

        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.SaleDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.SaleDate < dateTo.Value.Date.AddDays(1));
        }

        if (sellerUserId.HasValue)
        {
            query = query.Where(item => item.SellerUserId == sellerUserId.Value);
        }

        if (minAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount <= maxAmount.Value);
        }

        return View(new SaleIndexViewModel
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            SellerUserId = sellerUserId,
            MinAmount = minAmount,
            MaxAmount = maxAmount,
            Items = await query.OrderByDescending(item => item.SaleDate).ToListAsync(),
            Sellers = await LoadSellerOptionsAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var sale = await dbContext.Sales
            .AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.SellerUser)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.SaleId == id);

        return sale is null ? NotFound() : View(sale);
    }

    [PermissionAuthorize(AppPermissions.SalesManage)]
    public async Task<IActionResult> Create()
    {
        var model = await BuildViewModelAsync();
        if (User.GetUserId().HasValue)
        {
            model.SellerUserId = User.GetUserId()!.Value;
        }

        return View("Upsert", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SalesManage)]
    public async Task<IActionResult> Create(SaleEditViewModel model)
    {
        model.Items = NormalizeItems(model.Items);
        await FillLookupsAsync(model);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну позицию продажи.");
        }

        var errors = await inventoryService.ValidateSaleAsync(model.Items);
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var sale = new Sale
        {
            SaleDate = model.SaleDate,
            CustomerId = model.CustomerId,
            SellerUserId = model.SellerUserId,
            TotalAmount = model.Items.Sum(item => item.Quantity * item.UnitPrice),
            Items = model.Items.Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            }).ToList()
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.ApplySaleAsync(model.Items);
            dbContext.Sales.Add(sale);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Продажа оформлена.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Upsert", model);
        }
    }

    [PermissionAuthorize(AppPermissions.SalesManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var sale = await dbContext.Sales
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.SaleId == id);

        return sale is null ? NotFound() : View("Upsert", await BuildViewModelAsync(sale));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SalesManage)]
    public async Task<IActionResult> Edit(SaleEditViewModel model)
    {
        model.Items = NormalizeItems(model.Items);
        await FillLookupsAsync(model);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну позицию продажи.");
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var sale = await dbContext.Sales
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.SaleId == model.SaleId);

        if (sale is null)
        {
            return NotFound();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.RevertSaleAsync(sale.Items);
            dbContext.SaleItems.RemoveRange(sale.Items);

            var errors = await inventoryService.ValidateSaleAsync(model.Items);
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

            sale.SaleDate = model.SaleDate;
            sale.CustomerId = model.CustomerId;
            sale.SellerUserId = model.SellerUserId;
            sale.TotalAmount = model.Items.Sum(item => item.Quantity * item.UnitPrice);
            sale.Items = model.Items.Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            }).ToList();

            await inventoryService.ApplySaleAsync(model.Items);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Продажа обновлена.");
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

    [PermissionAuthorize(AppPermissions.SalesManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var sale = await dbContext.Sales
            .AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.SellerUser)
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.SaleId == id);

        return sale is null ? NotFound() : View(sale);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.SalesManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sale = await dbContext.Sales
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.SaleId == id);

        if (sale is null)
        {
            return NotFound();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.RevertSaleAsync(sale.Items);
            dbContext.Sales.Remove(sale);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            this.SetToastSuccess("Продажа удалена.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            this.SetToastError(exception.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(DateTime? dateFrom, DateTime? dateTo, int? sellerUserId, decimal? minAmount, decimal? maxAmount)
    {
        var query = dbContext.Sales.AsNoTracking().Include(item => item.Customer).Include(item => item.SellerUser).AsQueryable();
        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.SaleDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.SaleDate < dateTo.Value.Date.AddDays(1));
        }

        if (sellerUserId.HasValue)
        {
            query = query.Where(item => item.SellerUserId == sellerUserId.Value);
        }

        if (minAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(item => item.TotalAmount <= maxAmount.Value);
        }

        var sales = await query.OrderByDescending(item => item.SaleDate).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Продажи",
            ["ID", "Дата", "Клиент", "Продавец", "Сумма"],
            sales.Select(item => (IReadOnlyList<object?>)[item.SaleId, item.SaleDate, item.Customer?.FullName, item.SellerUser?.FullName, item.TotalAmount]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sales.xlsx");
    }

    private async Task<SaleEditViewModel> BuildViewModelAsync(Sale? sale = null)
    {
        var model = new SaleEditViewModel
        {
            SaleId = sale?.SaleId ?? 0,
            SaleDate = sale?.SaleDate ?? DateTime.Today,
            CustomerId = sale?.CustomerId,
            SellerUserId = sale?.SellerUserId ?? 0,
            Items = sale?.Items.Select(item => new SaleItemInputViewModel
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList() ?? [new()]
        };

        await FillLookupsAsync(model);
        return model;
    }

    private async Task FillLookupsAsync(SaleEditViewModel model)
    {
        model.Customers = await dbContext.Customers
            .AsNoTracking()
            .OrderBy(item => item.FullName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.CustomerId,
                Text = item.FullName
            })
            .ToListAsync();

        model.Sellers = await LoadSellerOptionsAsync();

        model.Products = await dbContext.Products
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.ProductName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.ProductId,
                Text = $"{item.ProductName} (остаток: {item.StockQuantity})"
            })
            .ToListAsync();
    }

    private async Task<List<SelectOptionViewModel>> LoadSellerOptionsAsync()
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .Where(item => item.IsActive && (item.Role!.RoleName == AppRoles.Seller || item.Role.RoleName == AppRoles.SystemAdministrator))
            .OrderBy(item => item.FullName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.UserId,
                Text = item.FullName
            })
            .ToListAsync();
    }

    private static List<SaleItemInputViewModel> NormalizeItems(IEnumerable<SaleItemInputViewModel> items)
    {
        return items
            .Where(item => item.ProductId > 0 && item.Quantity > 0 && item.UnitPrice > 0)
            .ToList();
    }
}
