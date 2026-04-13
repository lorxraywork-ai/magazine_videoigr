using System.Security.Claims;

namespace VideoGameStoreSystem.Web.Permissions;

public static class AppPermissions
{
    public const string DashboardView = "dashboard.view";
    public const string CatalogView = "catalog.view";
    public const string CategoriesView = "categories.view";
    public const string CategoriesManage = "categories.manage";
    public const string ProductsView = "products.view";
    public const string ProductsManage = "products.manage";
    public const string SuppliersView = "suppliers.view";
    public const string SuppliersManage = "suppliers.manage";
    public const string SuppliesView = "supplies.view";
    public const string SuppliesManage = "supplies.manage";
    public const string WriteOffsView = "writeoffs.view";
    public const string WriteOffsManage = "writeoffs.manage";
    public const string SalesView = "sales.view";
    public const string SalesManage = "sales.manage";
    public const string CustomersView = "customers.view";
    public const string CustomersManage = "customers.manage";
    public const string ReportsView = "reports.view";
    public const string FinanceReportsView = "reports.finance";
    public const string UsersView = "users.view";
    public const string UsersManage = "users.manage";
    public const string ChangeLogView = "changelog.view";
    public const string ProfileView = "profile.view";
    public const string PurchaseHistoryView = "purchasehistory.view";

    public static IReadOnlyList<string> All { get; } =
    [
        DashboardView,
        CatalogView,
        CategoriesView,
        CategoriesManage,
        ProductsView,
        ProductsManage,
        SuppliersView,
        SuppliersManage,
        SuppliesView,
        SuppliesManage,
        WriteOffsView,
        WriteOffsManage,
        SalesView,
        SalesManage,
        CustomersView,
        CustomersManage,
        ReportsView,
        FinanceReportsView,
        UsersView,
        UsersManage,
        ChangeLogView,
        ProfileView,
        PurchaseHistoryView
    ];

    public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> Matrix =
        new Dictionary<string, IReadOnlyCollection<string>>
        {
            [AppRoles.SystemAdministrator] = All,
            [AppRoles.Owner] =
            [
                DashboardView,
                CatalogView,
                CategoriesView,
                ProductsView,
                SuppliersView,
                SuppliesView,
                SalesView,
                CustomersView,
                ReportsView,
                FinanceReportsView,
                ProfileView
            ],
            [AppRoles.Manager] =
            [
                DashboardView,
                CatalogView,
                CategoriesView,
                CategoriesManage,
                ProductsView,
                ProductsManage,
                SuppliersView,
                SuppliersManage,
                SuppliesView,
                SuppliesManage,
                SalesView,
                CustomersView,
                ReportsView,
                ProfileView
            ],
            [AppRoles.Seller] =
            [
                DashboardView,
                CatalogView,
                ProductsView,
                SalesView,
                SalesManage,
                CustomersView,
                CustomersManage,
                ProfileView
            ],
            [AppRoles.Accountant] =
            [
                DashboardView,
                SalesView,
                CustomersView,
                ReportsView,
                FinanceReportsView,
                ProfileView
            ],
            [AppRoles.Warehouseman] =
            [
                DashboardView,
                CatalogView,
                ProductsView,
                SuppliersView,
                SuppliesView,
                SuppliesManage,
                WriteOffsView,
                WriteOffsManage,
                ProfileView
            ],
            [AppRoles.Client] =
            [
                CatalogView,
                ProfileView,
                PurchaseHistoryView
            ]
        };

    public static bool HasPermission(string? roleName, string permission)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return false;
        }

        return Matrix.TryGetValue(roleName, out var permissions) && permissions.Contains(permission);
    }
}

public static class UserPermissionExtensions
{
    public static bool HasPermission(this ClaimsPrincipal user, string permission)
    {
        if (!(user.Identity?.IsAuthenticated ?? false))
        {
            return false;
        }

        return AppPermissions.HasPermission(user.FindFirstValue(ClaimTypes.Role), permission);
    }
}
