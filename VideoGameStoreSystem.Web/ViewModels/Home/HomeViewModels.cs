namespace VideoGameStoreSystem.Web.ViewModels.Home;

public class DashboardCardViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = "Index";

    public string Icon { get; set; } = "bi-grid";

    public string AccentClass { get; set; } = "primary";
}

public class DashboardViewModel
{
    public string? Login { get; set; }

    public string? FullName { get; set; }

    public string? RoleName { get; set; }

    public int ProductCount { get; set; }

    public int LowStockCount { get; set; }

    public int SaleCount { get; set; }

    public decimal RevenueTotal { get; set; }

    public List<DashboardCardViewModel> Cards { get; set; } = [];
}
