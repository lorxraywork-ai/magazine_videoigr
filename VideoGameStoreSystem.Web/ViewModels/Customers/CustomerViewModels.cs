using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.Customers;

public class CustomerIndexViewModel
{
    public string? Search { get; set; }

    public List<Customer> Items { get; set; } = [];
}

public class CustomerFormViewModel
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Введите ФИО клиента.")]
    [StringLength(150)]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(150)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Связанный пользователь")]
    public int? UserId { get; set; }

    public List<SelectOptionViewModel> Users { get; set; } = [];
}
