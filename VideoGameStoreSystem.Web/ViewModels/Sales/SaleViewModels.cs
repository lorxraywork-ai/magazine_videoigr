using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.Sales;

public class SaleIndexViewModel
{
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public int? SellerUserId { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public List<Sale> Items { get; set; } = [];

    public List<SelectOptionViewModel> Sellers { get; set; } = [];
}

public class SaleEditViewModel
{
    public int SaleId { get; set; }

    [Display(Name = "Дата продажи")]
    public DateTime SaleDate { get; set; } = DateTime.Today;

    [Display(Name = "Клиент")]
    public int? CustomerId { get; set; }

    [Required(ErrorMessage = "Выберите продавца.")]
    [Display(Name = "Продавец")]
    public int SellerUserId { get; set; }

    public List<SaleItemInputViewModel> Items { get; set; } = [new()];

    public List<SelectOptionViewModel> Customers { get; set; } = [];

    public List<SelectOptionViewModel> Sellers { get; set; } = [];

    public List<SelectOptionViewModel> Products { get; set; } = [];
}

public class SaleItemInputViewModel
{
    [Required(ErrorMessage = "Выберите товар.")]
    [Display(Name = "Товар")]
    public int ProductId { get; set; }

    [Range(1, 10000)]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    [Range(0.01, 999999)]
    [Display(Name = "Цена продажи")]
    public decimal UnitPrice { get; set; }
}
