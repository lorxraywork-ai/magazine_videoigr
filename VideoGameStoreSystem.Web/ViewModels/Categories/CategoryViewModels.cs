using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;

namespace VideoGameStoreSystem.Web.ViewModels.Categories;

public class CategoryIndexViewModel
{
    public string? Search { get; set; }

    public List<Category> Items { get; set; } = [];
}

public class CategoryFormViewModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Введите название категории.")]
    [StringLength(100)]
    [Display(Name = "Название категории")]
    public string CategoryName { get; set; } = string.Empty;
}
