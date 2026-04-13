using System.ComponentModel.DataAnnotations;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Common;

namespace VideoGameStoreSystem.Web.ViewModels.WriteOffs;

public class WriteOffIndexViewModel
{
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? Reason { get; set; }

    public List<WriteOff> Items { get; set; } = [];
}

public class WriteOffEditViewModel
{
    public int WriteOffId { get; set; }

    [Display(Name = "Дата списания")]
    public DateTime WriteOffDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Укажите причину списания.")]
    [StringLength(300)]
    [Display(Name = "Причина")]
    public string Reason { get; set; } = string.Empty;

    public List<WriteOffItemInputViewModel> Items { get; set; } = [new()];

    public List<SelectOptionViewModel> Products { get; set; } = [];
}

public class WriteOffItemInputViewModel
{
    [Required(ErrorMessage = "Выберите товар.")]
    [Display(Name = "Товар")]
    public int ProductId { get; set; }

    [Range(1, 10000)]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }
}
