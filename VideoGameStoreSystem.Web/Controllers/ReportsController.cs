using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Reports;

namespace VideoGameStoreSystem.Web.Controllers;

[Authorize]
[PermissionAuthorize(AppPermissions.ReportsView)]
public class ReportsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
    {
        var model = await BuildSalesProfitReportAsync(dateFrom ?? DateTime.Today.AddDays(-30), dateTo ?? DateTime.Today);
        return View(model);
    }

    public async Task<FileResult> Export(DateTime? dateFrom, DateTime? dateTo)
    {
        var report = await BuildSalesProfitReportAsync(dateFrom ?? DateTime.Today.AddDays(-30), dateTo ?? DateTime.Today);
        var rows = new List<IReadOnlyList<object?>>
        {
            new object?[] { "Период с", report.DateFrom, null, null },
            new object?[] { "Период по", report.DateTo, null, null },
            new object?[] { "Количество продаж", report.SaleCount, null, null },
            new object?[] { "Общая выручка", report.RevenueTotal, null, null },
            new object?[] { "Предполагаемая прибыль", report.ProfitTotal, null, null }
        };

        rows.AddRange(report.TopProducts.Select(item => (IReadOnlyList<object?>)
        [
            $"Топ товар: {item.ProductName}",
            item.Quantity,
            item.Revenue,
            item.Profit
        ]));

        rows.AddRange(report.CategorySales.Select(item => (IReadOnlyList<object?>)
        [
            $"Категория: {item.CategoryName}",
            item.Quantity,
            item.Revenue,
            null
        ]));

        var bytes = ExcelExportHelper.BuildWorkbook(
            "Отчет по продажам и прибыли",
            ["Показатель", "Значение 1", "Значение 2", "Значение 3"],
            rows);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sales-report.xlsx");
    }

    private async Task<SalesProfitReportViewModel> BuildSalesProfitReportAsync(DateTime dateFrom, DateTime dateTo)
    {
        var normalizedFrom = dateFrom.Date;
        var normalizedTo = dateTo.Date.AddDays(1);

        var sales = await dbContext.Sales
            .AsNoTracking()
            .Include(item => item.Items)
                .ThenInclude(item => item.Product)
                    .ThenInclude(item => item!.Category)
            .Where(item => item.SaleDate >= normalizedFrom && item.SaleDate < normalizedTo)
            .ToListAsync();

        var topProducts = sales
            .SelectMany(item => item.Items)
            .GroupBy(item => item.Product!.ProductName)
            .Select(group => new TopProductReportItemViewModel
            {
                ProductName = group.Key,
                Quantity = group.Sum(item => item.Quantity),
                Revenue = group.Sum(item => item.LineTotal),
                Profit = group.Sum(item => (item.UnitPrice - item.Product!.CostPrice) * item.Quantity)
            })
            .OrderByDescending(item => item.Quantity)
            .ThenByDescending(item => item.Revenue)
            .Take(10)
            .ToList();

        var categorySales = sales
            .SelectMany(item => item.Items)
            .GroupBy(item => item.Product!.Category!.CategoryName)
            .Select(group => new CategorySalesReportItemViewModel
            {
                CategoryName = group.Key,
                Quantity = group.Sum(item => item.Quantity),
                Revenue = group.Sum(item => item.LineTotal)
            })
            .OrderByDescending(item => item.Revenue)
            .ToList();

        return new SalesProfitReportViewModel
        {
            DateFrom = dateFrom.Date,
            DateTo = dateTo.Date,
            SaleCount = sales.Count,
            RevenueTotal = sales.Sum(item => item.TotalAmount),
            ProfitTotal = sales.SelectMany(item => item.Items).Sum(item => (item.UnitPrice - item.Product!.CostPrice) * item.Quantity),
            TopProducts = topProducts,
            CategorySales = categorySales
        };
    }
}
