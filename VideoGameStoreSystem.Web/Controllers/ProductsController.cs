using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.Products;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.ProductsView)]
public class ProductsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? isActive)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.ProductName.Contains(search) || item.Sku.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(item => item.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(item => item.Price <= maxPrice.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(item => item.IsActive == isActive.Value);
        }

        return View(new ProductIndexViewModel
        {
            Search = search,
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            IsActive = isActive,
            Items = await query.OrderBy(item => item.ProductName).ToListAsync(),
            Categories = await LoadCategoryOptionsAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.ProductId == id);

        return product is null ? NotFound() : View(product);
    }

    [PermissionAuthorize(AppPermissions.ProductsManage)]
    public async Task<IActionResult> Create()
    {
        return View("Upsert", new ProductFormViewModel
        {
            Categories = await LoadCategoryOptionsAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.ProductsManage)]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        model.Categories = await LoadCategoryOptionsAsync();
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        if (await dbContext.Products.AnyAsync(item => item.Sku == model.Sku))
        {
            ModelState.AddModelError(nameof(model.Sku), "Товар с таким артикулом уже существует.");
            return View("Upsert", model);
        }

        dbContext.Products.Add(new Product
        {
            ProductName = model.ProductName,
            CategoryId = model.CategoryId,
            Sku = model.Sku,
            Description = model.Description,
            Price = model.Price,
            CostPrice = model.CostPrice,
            IsActive = model.IsActive
        });

        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Товар добавлен.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.ProductsManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return View("Upsert", new ProductFormViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            Sku = product.Sku,
            Description = product.Description,
            Price = product.Price,
            CostPrice = product.CostPrice,
            IsActive = product.IsActive,
            Categories = await LoadCategoryOptionsAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.ProductsManage)]
    public async Task<IActionResult> Edit(ProductFormViewModel model)
    {
        model.Categories = await LoadCategoryOptionsAsync();
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var product = await dbContext.Products.FindAsync(model.ProductId);
        if (product is null)
        {
            return NotFound();
        }

        if (await dbContext.Products.AnyAsync(item =>
                item.Sku == model.Sku &&
                item.ProductId != model.ProductId))
        {
            ModelState.AddModelError(nameof(model.Sku), "Товар с таким артикулом уже существует.");
            return View("Upsert", model);
        }

        product.ProductName = model.ProductName;
        product.CategoryId = model.CategoryId;
        product.Sku = model.Sku;
        product.Description = model.Description;
        product.Price = model.Price;
        product.CostPrice = model.CostPrice;
        product.IsActive = model.IsActive;

        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Данные товара обновлены.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.ProductsManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.ProductId == id);

        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.ProductsManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await dbContext.Products
            .Include(item => item.SupplyItems)
            .Include(item => item.SaleItems)
            .Include(item => item.WriteOffItems)
            .FirstOrDefaultAsync(item => item.ProductId == id);

        if (product is null)
        {
            return NotFound();
        }

        if (product.SupplyItems.Count > 0 || product.SaleItems.Count > 0 || product.WriteOffItems.Count > 0)
        {
            this.SetToastError("Нельзя удалить товар, который участвует в документах.");
            return RedirectToAction(nameof(Index));
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Товар удален.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? isActive)
    {
        var query = dbContext.Products.AsNoTracking().Include(item => item.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.ProductName.Contains(search) || item.Sku.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(item => item.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(item => item.Price <= maxPrice.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(item => item.IsActive == isActive.Value);
        }

        var products = await query.OrderBy(item => item.ProductName).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Товары магазина видеоигр",
            ["ID", "Название", "Категория", "SKU", "Цена", "Себестоимость", "Остаток", "Активен"],
            products.Select(item => (IReadOnlyList<object?>)
            [
                item.ProductId,
                item.ProductName,
                item.Category?.CategoryName,
                item.Sku,
                item.Price,
                item.CostPrice,
                item.StockQuantity,
                item.IsActive ? "Да" : "Нет"
            ]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
    }

    private async Task<List<SelectOptionViewModel>> LoadCategoryOptionsAsync()
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(item => item.CategoryName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.CategoryId,
                Text = item.CategoryName
            })
            .ToListAsync();
    }
}
