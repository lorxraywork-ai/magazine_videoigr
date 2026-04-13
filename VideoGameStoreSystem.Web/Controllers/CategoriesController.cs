using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Categories;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.CategoriesView)]
public class CategoriesController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = dbContext.Categories.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.CategoryName.Contains(search));
        }

        return View(new CategoryIndexViewModel
        {
            Search = search,
            Items = await query.OrderBy(item => item.CategoryName).ToListAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .Include(item => item.Products)
            .FirstOrDefaultAsync(item => item.CategoryId == id);

        return category is null ? NotFound() : View(category);
    }

    [PermissionAuthorize(AppPermissions.CategoriesManage)]
    public IActionResult Create()
    {
        return View("Upsert", new CategoryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.CategoriesManage)]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        if (await dbContext.Categories.AnyAsync(item => item.CategoryName == model.CategoryName))
        {
            ModelState.AddModelError(nameof(model.CategoryName), "Категория с таким названием уже существует.");
            return View("Upsert", model);
        }

        dbContext.Categories.Add(new Category { CategoryName = model.CategoryName });
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Категория добавлена.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.CategoriesManage)]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        return View("Upsert", new CategoryFormViewModel
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.CategoriesManage)]
    public async Task<IActionResult> Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Upsert", model);
        }

        var category = await dbContext.Categories.FindAsync(model.CategoryId);
        if (category is null)
        {
            return NotFound();
        }

        if (await dbContext.Categories.AnyAsync(item =>
                item.CategoryName == model.CategoryName &&
                item.CategoryId != model.CategoryId))
        {
            ModelState.AddModelError(nameof(model.CategoryName), "Категория с таким названием уже существует.");
            return View("Upsert", model);
        }

        category.CategoryName = model.CategoryName;
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Категория обновлена.");
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(AppPermissions.CategoriesManage)]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .Include(item => item.Products)
            .FirstOrDefaultAsync(item => item.CategoryId == id);

        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(AppPermissions.CategoriesManage)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await dbContext.Categories
            .Include(item => item.Products)
            .FirstOrDefaultAsync(item => item.CategoryId == id);

        if (category is null)
        {
            return NotFound();
        }

        if (category.Products.Count > 0)
        {
            this.SetToastError("Нельзя удалить категорию, в которой есть товары.");
            return RedirectToAction(nameof(Index));
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
        this.SetToastSuccess("Категория удалена.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<FileResult> Export(string? search)
    {
        var query = dbContext.Categories.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.CategoryName.Contains(search));
        }

        var categories = await query.OrderBy(item => item.CategoryName).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Категории товаров",
            ["ID", "Категория"],
            categories.Select(item => (IReadOnlyList<object?>)[item.CategoryId, item.CategoryName]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "categories.xlsx");
    }
}
