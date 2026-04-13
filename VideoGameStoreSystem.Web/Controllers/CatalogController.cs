using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.ViewModels.Catalog;

namespace VideoGameStoreSystem.Web.Controllers;

[AllowAnonymous]
public class CatalogController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search, int? categoryId, bool onlyActive = true)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .AsQueryable();

        if (onlyActive)
        {
            query = query.Where(item => item.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.ProductName.Contains(search) || item.Sku.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == categoryId.Value);
        }

        var model = new CatalogIndexViewModel
        {
            Search = search,
            CategoryId = categoryId,
            OnlyActive = onlyActive,
            Products = await query
                .OrderBy(item => item.ProductName)
                .Select(item => new CatalogProductCardViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    CategoryName = item.Category!.CategoryName,
                    Sku = item.Sku,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    IsActive = item.IsActive
                })
                .ToListAsync(),
            Categories = await dbContext.Categories
                .AsNoTracking()
                .OrderBy(item => item.CategoryName)
                .Select(item => new KeyValuePair<int, string>(item.CategoryId, item.CategoryName))
                .ToListAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .Where(item => item.ProductId == id)
            .Select(item => new CatalogDetailsViewModel
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                CategoryName = item.Category!.CategoryName,
                Sku = item.Sku,
                Description = item.Description,
                Price = item.Price,
                CostPrice = item.CostPrice,
                StockQuantity = item.StockQuantity,
                IsActive = item.IsActive
            })
            .FirstOrDefaultAsync();

        return product is null ? NotFound() : View(product);
    }
}
