using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.Cart;

public class CartIndexViewModel
{
    public List<CartLineViewModel> Items { get; set; } = [];

    [Display(Name = "Продавец")]
    public int? SelectedSellerUserId { get; set; }

    public List<SelectOptionViewModel> Sellers { get; set; } = [];

    public string? CustomerDisplayName { get; set; }

    public int TotalQuantity => Items.Sum(item => item.Quantity);

    public decimal TotalAmount => Items.Sum(item => item.LineTotal);

    public bool CanCheckout =>
        SelectedSellerUserId.HasValue &&
        Items.Count > 0 &&
        Items.All(item => item.IsActive && item.StockQuantity >= item.Quantity);
}

public class CartLineViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public decimal LineTotal => Price * Quantity;
}
