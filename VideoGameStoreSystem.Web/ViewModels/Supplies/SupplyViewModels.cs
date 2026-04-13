using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.Supplies;

public class SupplyIndexViewModel
{
    public int? SupplierId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public List<Supply> Items { get; set; } = [];

    public List<SelectOptionViewModel> Suppliers { get; set; } = [];
}

public class SupplyEditViewModel
{
    public int SupplyId { get; set; }

    [Display(Name = "Дата поступления")]
    public DateTime SupplyDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Выберите поставщика.")]
    [Display(Name = "Поставщик")]
    public int SupplierId { get; set; }

    public List<SupplyItemInputViewModel> Items { get; set; } = [new()];

    public List<SelectOptionViewModel> Suppliers { get; set; } = [];

    public List<SelectOptionViewModel> Products { get; set; } = [];
}

public class SupplyItemInputViewModel
{
    [Required(ErrorMessage = "Выберите товар.")]
    [Display(Name = "Товар")]
    public int ProductId { get; set; }

    [Range(1, 10000)]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    [Range(0.01, 999999)]
    [Display(Name = "Цена закупки")]
    public decimal UnitCost { get; set; }
}
