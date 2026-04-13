namespace VideoGameStoreSystem.Web.ViewModels.Catalog;

public class CatalogIndexViewModel
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public bool OnlyActive { get; set; } = true;

    public List<CatalogProductCardViewModel> Products { get; set; } = [];

    public List<KeyValuePair<int, string>> Categories { get; set; } = [];
}

public class CatalogProductCardViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }
}

public class CatalogDetailsViewModel : CatalogProductCardViewModel
{
    public decimal CostPrice { get; set; }
}
