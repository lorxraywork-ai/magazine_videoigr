using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.ChangeLog;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.ChangeLogView)]
public class ChangeLogController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search, string? actionType, DateTime? dateFrom, DateTime? dateTo)
    {
        var query = dbContext.ChangeLogs
            .AsNoTracking()
            .Include(item => item.ChangedByUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.EntityName.Contains(search) ||
                item.EntityId.Contains(search) ||
                item.NewValues!.Contains(search) ||
                item.OldValues!.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(actionType))
        {
            query = query.Where(item => item.ActionType == actionType);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.ChangedAt >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.ChangedAt < dateTo.Value.Date.AddDays(1));
        }

        return View(new ChangeLogIndexViewModel
        {
            Search = search,
            ActionType = actionType,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Items = await query.OrderByDescending(item => item.ChangedAt).ToListAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await dbContext.ChangeLogs
            .AsNoTracking()
            .Include(log => log.ChangedByUser)
            .FirstOrDefaultAsync(log => log.ChangeLogId == id);

        return item is null ? NotFound() : View(item);
    }

    public async Task<FileResult> Export(string? search, string? actionType, DateTime? dateFrom, DateTime? dateTo)
    {
        var query = dbContext.ChangeLogs.AsNoTracking().Include(item => item.ChangedByUser).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.EntityName.Contains(search) ||
                item.EntityId.Contains(search) ||
                (item.NewValues != null && item.NewValues.Contains(search)) ||
                (item.OldValues != null && item.OldValues.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(actionType))
        {
            query = query.Where(item => item.ActionType == actionType);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(item => item.ChangedAt >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(item => item.ChangedAt < dateTo.Value.Date.AddDays(1));
        }

        var logs = await query.OrderByDescending(item => item.ChangedAt).ToListAsync();
        var bytes = ExcelExportHelper.BuildWorkbook(
            "Журнал изменений",
            ["ID", "Сущность", "Ключ", "Действие", "Старые значения", "Новые значения", "Дата", "Пользователь"],
            logs.Select(item => (IReadOnlyList<object?>)
            [
                item.ChangeLogId,
                item.EntityName,
                item.EntityId,
                item.ActionType,
                item.OldValues,
                item.NewValues,
                item.ChangedAt,
                item.ChangedByUser?.Login ?? "Система"
            ]));

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "change-log.xlsx");
    }
}
