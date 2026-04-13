using VideoGameStoreSystem.Web.Models;

namespace VideoGameStoreSystem.Web.ViewModels.Profile;

public class ProfileViewModel
{
    public string Login { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Customer? CustomerProfile { get; set; }
}

public class PurchaseHistoryViewModel
{
    public List<Sale> Sales { get; set; } = [];
}
