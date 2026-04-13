using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;

namespace VideoGameStoreSystem.Web.ViewModels.Suppliers;

public class SupplierIndexViewModel
{
    public string? Search { get; set; }

    public List<Supplier> Items { get; set; } = [];
}

public class SupplierFormViewModel
{
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Введите название поставщика.")]
    [StringLength(150)]
    [Display(Name = "Поставщик")]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(150)]
    [Display(Name = "Email")]
    public string? Email { get; set; }
}
