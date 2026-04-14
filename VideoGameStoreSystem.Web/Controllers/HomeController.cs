using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Home;

namespace VideoGameStoreSystem.Web.Controllers;

public class HomeController(ApplicationDbContext dbContext) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            Login = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость",
            FullName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Неавторизованный пользователь",
            RoleName = User.FindFirstValue(ClaimTypes.Role) ?? "Нет роли"
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            model.ShowSummaryStatistics = !User.IsInRole(AppRoles.Client);

            if (model.ShowSummaryStatistics)
            {
                model.ProductCount = await dbContext.Products.CountAsync();
                model.LowStockCount = await dbContext.Products.CountAsync(item => item.StockQuantity <= 2);
                model.SaleCount = await dbContext.Sales.CountAsync();
                model.RevenueTotal = await dbContext.Sales.SumAsync(item => (decimal?)item.TotalAmount) ?? 0m;
            }

            model.Cards = BuildAuthorizedCards(User);
        }
        else
        {
            model.Cards =
            [
                new DashboardCardViewModel { Title = "Каталог товаров", Description = "Открытый просмотр ассортимента магазина видеоигр.", Controller = "Catalog", Icon = "bi-controller", AccentClass = "primary" },
                new DashboardCardViewModel { Title = "Вход в систему", Description = "Авторизация сотрудников и клиентов магазина.", Controller = "Account", Action = "Login", Icon = "bi-box-arrow-in-right", AccentClass = "info" },
                new DashboardCardViewModel { Title = "Регистрация клиента", Description = "Создание клиентского аккаунта для покупок и истории заказов.", Controller = "Account", Action = "Register", Icon = "bi-person-plus", AccentClass = "success" }
            ];
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static List<DashboardCardViewModel> BuildAuthorizedCards(ClaimsPrincipal user)
    {
        var cards = new List<(string Permission, DashboardCardViewModel Card)>
        {
            (AppPermissions.CatalogView, new DashboardCardViewModel { Title = "Каталог", Description = "Витрина товаров магазина видеоигр.", Controller = "Catalog", Icon = "bi-controller", AccentClass = "primary" }),
            (AppPermissions.ProductsView, new DashboardCardViewModel { Title = "Товары", Description = "Управление ассортиментом, ценами и остатками.", Controller = "Products", Icon = "bi-bag", AccentClass = "primary" }),
            (AppPermissions.CategoriesView, new DashboardCardViewModel { Title = "Категории", Description = "Группы товаров и структура каталога.", Controller = "Categories", Icon = "bi-collection", AccentClass = "info" }),
            (AppPermissions.SuppliersView, new DashboardCardViewModel { Title = "Поставщики", Description = "Контакты и база контрагентов.", Controller = "Suppliers", Icon = "bi-truck", AccentClass = "info" }),
            (AppPermissions.SuppliesView, new DashboardCardViewModel { Title = "Поступления", Description = "Документы приемки и пополнение склада.", Controller = "Supplies", Icon = "bi-box-seam", AccentClass = "success" }),
            (AppPermissions.WriteOffsView, new DashboardCardViewModel { Title = "Списания", Description = "Фиксация потерь и убытков по складу.", Controller = "WriteOffs", Icon = "bi-exclamation-triangle", AccentClass = "warning" }),
            (AppPermissions.SalesView, new DashboardCardViewModel { Title = "Продажи", Description = "Оформление продаж и история чеков.", Controller = "Sales", Icon = "bi-receipt", AccentClass = "success" }),
            (AppPermissions.CustomersView, new DashboardCardViewModel { Title = "Клиенты", Description = "Покупатели и история взаимодействия.", Controller = "Customers", Icon = "bi-people", AccentClass = "primary" }),
            (AppPermissions.ReportsView, new DashboardCardViewModel { Title = "Отчеты", Description = "Продажи, выручка, прибыль и аналитика.", Controller = "Reports", Icon = "bi-bar-chart", AccentClass = "dark" }),
            (AppPermissions.UsersView, new DashboardCardViewModel { Title = "Пользователи", Description = "Учетные записи сотрудников и клиентов.", Controller = "Users", Icon = "bi-person-badge", AccentClass = "dark" }),
            (AppPermissions.ChangeLogView, new DashboardCardViewModel { Title = "Журнал изменений", Description = "Аудит операций в системе.", Controller = "ChangeLog", Icon = "bi-clock-history", AccentClass = "secondary" })
        };

        return cards
            .Where(item => user.HasPermission(item.Permission))
            .Select(item => item.Card)
            .ToList();
    }
}
