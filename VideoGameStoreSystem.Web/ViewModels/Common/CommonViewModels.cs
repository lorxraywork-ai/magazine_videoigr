using Microsoft.AspNetCore.Mvc.Rendering;

namespace VideoGameStoreSystem.Web.ViewModels.Common;

public class SelectOptionViewModel
{
    public int Value { get; set; }

    public string Text { get; set; } = string.Empty;
}

public class DeleteViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public string ReturnAction { get; set; } = "Index";
}

public static class SelectListExtensions
{
    public static List<SelectListItem> ToSelectList(
        this IEnumerable<SelectOptionViewModel> options,
        int? selectedValue = null)
    {
        return options.Select(item => new SelectListItem
        {
            Value = item.Value.ToString(),
            Text = item.Text,
            Selected = selectedValue.HasValue && item.Value == selectedValue.Value
        }).ToList();
    }
}
