using System.ComponentModel.DataAnnotations;

namespace VideoGameStoreSystem.Web.Models;

public class AppRole
{
    public int RoleId { get; set; }

    [Required, StringLength(100)]
    public string RoleName { get; set; } = string.Empty;

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}

public class AppUser
{
    public int UserId { get; set; }

    [Required, StringLength(50)]
    public string Login { get; set; } = string.Empty;

    [Required, StringLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public AppRole? Role { get; set; }

    public Customer? CustomerProfile { get; set; }

    public ICollection<Sale> SalesAsSeller { get; set; } = new List<Sale>();

    public ICollection<WriteOff> WriteOffsCreated { get; set; } = new List<WriteOff>();

    public ICollection<AppChangeLog> Changes { get; set; } = new List<AppChangeLog>();
}

public class AppChangeLog
{
    public int ChangeLogId { get; set; }

    [Required, StringLength(150)]
    public string EntityName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string EntityId { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string ActionType { get; set; } = string.Empty;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.Now;

    public int? ChangedByUserId { get; set; }

    public AppUser? ChangedByUser { get; set; }
}
