using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.Products;

public class ProductIndexViewModel
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public bool? IsActive { get; set; }

    public List<Product> Items { get; set; } = [];

    public List<SelectOptionViewModel> Categories { get; set; } = [];
}

public class ProductFormViewModel
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Введите название товара.")]
    [StringLength(200)]
    [Display(Name = "Название товара")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите категорию.")]
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Введите SKU.")]
    [StringLength(40)]
    [Display(Name = "Артикул (SKU)")]
    public string Sku { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Range(0, 999999)]
    [Display(Name = "Цена продажи")]
    public decimal Price { get; set; }

    [Range(0, 999999)]
    [Display(Name = "Себестоимость")]
    public decimal CostPrice { get; set; }

    [Display(Name = "Активен")]
    public bool IsActive { get; set; } = true;

    public List<SelectOptionViewModel> Categories { get; set; } = [];
}
