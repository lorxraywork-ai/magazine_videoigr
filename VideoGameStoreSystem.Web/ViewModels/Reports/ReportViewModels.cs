namespace VideoGameStoreSystem.Web.ViewModels.Reports;

public class SalesProfitReportViewModel
{
    public DateTime DateFrom { get; set; } = DateTime.Today.AddDays(-30);

    public DateTime DateTo { get; set; } = DateTime.Today;

    public int SaleCount { get; set; }

    public decimal RevenueTotal { get; set; }

    public decimal ProfitTotal { get; set; }

    public List<TopProductReportItemViewModel> TopProducts { get; set; } = [];

    public List<CategorySalesReportItemViewModel> CategorySales { get; set; } = [];
}

public class TopProductReportItemViewModel
{
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Revenue { get; set; }

    public decimal Profit { get; set; }
}

public class CategorySalesReportItemViewModel
{
    public string CategoryName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Revenue { get; set; }
}
