using VideoGameStoreSystem.Web.Models;

namespace VideoGameStoreSystem.Web.ViewModels.ChangeLog;

public class ChangeLogIndexViewModel
{
    public string? Search { get; set; }

    public string? ActionType { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public List<AppChangeLog> Items { get; set; } = [];
}
