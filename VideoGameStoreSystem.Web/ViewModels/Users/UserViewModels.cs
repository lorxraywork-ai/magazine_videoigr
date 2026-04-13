using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.Users;

public class UserIndexViewModel
{
    public string? Search { get; set; }

    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }

    public List<AppUser> Items { get; set; } = [];

    public List<SelectOptionViewModel> Roles { get; set; } = [];
}

public class UserFormViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Введите логин.")]
    [StringLength(50)]
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите ФИО.")]
    [StringLength(150)]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email.")]
    [EmailAddress]
    [StringLength(150)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите роль.")]
    [Display(Name = "Роль")]
    public int RoleId { get; set; }

    [Display(Name = "Активен")]
    public bool IsActive { get; set; } = true;

    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; }

    public List<SelectOptionViewModel> Roles { get; set; } = [];
}
