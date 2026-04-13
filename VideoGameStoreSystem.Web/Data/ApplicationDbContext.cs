using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;

namespace VideoGameStoreSystem.Web.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ICurrentUserService currentUserService) : DbContext(options)
{
    private bool _savingAuditEntries;

    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppChangeLog> ChangeLogs => Set<AppChangeLog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Supply> Supplies => Set<Supply>();
    public DbSet<SupplyItem> SupplyItems => Set<SupplyItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<WriteOff> WriteOffs => Set<WriteOff>();
    public DbSet<WriteOffItem> WriteOffItems => Set<WriteOffItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.ToTable("APP_ROLE");
            entity.HasKey(item => item.RoleId);
            entity.HasIndex(item => item.RoleName).IsUnique();
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("APP_USER");
            entity.HasKey(item => item.UserId);
            entity.HasIndex(item => item.Login).IsUnique();
            entity.HasIndex(item => item.Email);
            entity.HasOne(item => item.Role)
                .WithMany(item => item.Users)
                .HasForeignKey(item => item.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppChangeLog>(entity =>
        {
            entity.ToTable("APP_CHANGE_LOG");
            entity.HasKey(item => item.ChangeLogId);
            entity.HasIndex(item => item.ChangedAt);
            entity.HasOne(item => item.ChangedByUser)
                .WithMany(item => item.Changes)
                .HasForeignKey(item => item.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("CATEGORY");
            entity.HasKey(item => item.CategoryId);
            entity.HasIndex(item => item.CategoryName).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("PRODUCT");
            entity.HasKey(item => item.ProductId);
            entity.HasIndex(item => item.ProductName);
            entity.HasIndex(item => item.Sku).IsUnique();
            entity.HasIndex(item => item.CategoryId);
            entity.Property(item => item.Price).HasColumnType("decimal(18,2)");
            entity.Property(item => item.CostPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(item => item.Category)
                .WithMany(item => item.Products)
                .HasForeignKey(item => item.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("SUPPLIER");
            entity.HasKey(item => item.SupplierId);
            entity.HasIndex(item => item.SupplierName);
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.ToTable("SUPPLY");
            entity.HasKey(item => item.SupplyId);
            entity.HasIndex(item => item.SupplyDate);
            entity.Property(item => item.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(item => item.Supplier)
                .WithMany(item => item.Supplies)
                .HasForeignKey(item => item.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupplyItem>(entity =>
        {
            entity.ToTable("SUPPLY_ITEM");
            entity.HasKey(item => item.SupplyItemId);
            entity.Property(item => item.UnitCost).HasColumnType("decimal(18,2)");
            entity.HasOne(item => item.Supply)
                .WithMany(item => item.Items)
                .HasForeignKey(item => item.SupplyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Product)
                .WithMany(item => item.SupplyItems)
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("CUSTOMER");
            entity.HasKey(item => item.CustomerId);
            entity.HasIndex(item => item.FullName);
            entity.HasIndex(item => item.Email);
            entity.HasOne(item => item.User)
                .WithOne(item => item.CustomerProfile)
                .HasForeignKey<Customer>(item => item.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("SALE");
            entity.HasKey(item => item.SaleId);
            entity.HasIndex(item => item.SaleDate);
            entity.Property(item => item.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(item => item.Customer)
                .WithMany(item => item.Sales)
                .HasForeignKey(item => item.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(item => item.SellerUser)
                .WithMany(item => item.SalesAsSeller)
                .HasForeignKey(item => item.SellerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("SALE_ITEM");
            entity.HasKey(item => item.SaleItemId);
            entity.Property(item => item.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(item => item.LineTotal).HasColumnType("decimal(18,2)");
            entity.HasOne(item => item.Sale)
                .WithMany(item => item.Items)
                .HasForeignKey(item => item.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Product)
                .WithMany(item => item.SaleItems)
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WriteOff>(entity =>
        {
            entity.ToTable("WRITE_OFF");
            entity.HasKey(item => item.WriteOffId);
            entity.HasIndex(item => item.WriteOffDate);
            entity.HasOne(item => item.CreatedByUser)
                .WithMany(item => item.WriteOffsCreated)
                .HasForeignKey(item => item.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WriteOffItem>(entity =>
        {
            entity.ToTable("WRITE_OFF_ITEM");
            entity.HasKey(item => item.WriteOffItemId);
            entity.HasOne(item => item.WriteOff)
                .WithMany(item => item.Items)
                .HasForeignKey(item => item.WriteOffId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Product)
                .WithMany(item => item.WriteOffItems)
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.ApplyUpperSnakeCaseNaming();
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_savingAuditEntries)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var auditEntries = PrepareAuditEntries();
        var result = await base.SaveChangesAsync(cancellationToken);

        if (auditEntries.Count > 0)
        {
            await SaveAuditEntriesAsync(auditEntries, cancellationToken);
        }

        return result;
    }

    private List<PendingAuditEntry> PrepareAuditEntries()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<PendingAuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AppChangeLog || entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            var auditEntry = new PendingAuditEntry
            {
                ActionType = entry.State switch
                {
                    EntityState.Added => "CREATE",
                    EntityState.Modified => "UPDATE",
                    EntityState.Deleted => "DELETE",
                    _ => entry.State.ToString().ToUpperInvariant()
                },
                EntityName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                ChangedByUserId = currentUserService.UserId
            };

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[property.Metadata.Name] = property.CurrentValue;
                    continue;
                }

                if (property.Metadata.Name == nameof(AppUser.PasswordHash))
                {
                    continue;
                }

                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                if (entry.State == EntityState.Added)
                {
                    auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                    continue;
                }

                if (entry.State == EntityState.Deleted)
                {
                    auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                    continue;
                }

                if (property.IsModified && !Equals(property.OriginalValue, property.CurrentValue))
                {
                    auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                    auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                }
            }

            if (!auditEntry.HasChanges)
            {
                continue;
            }

            auditEntries.Add(auditEntry);
        }

        return auditEntries;
    }

    private async Task SaveAuditEntriesAsync(
        IEnumerable<PendingAuditEntry> pendingAuditEntries,
        CancellationToken cancellationToken)
    {
        _savingAuditEntries = true;

        foreach (var pendingAuditEntry in pendingAuditEntries)
        {
            foreach (var property in pendingAuditEntry.TemporaryProperties)
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    pendingAuditEntry.KeyValues[property.Metadata.Name] = property.CurrentValue;
                }
                else
                {
                    pendingAuditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                }
            }

            ChangeLogs.Add(pendingAuditEntry.ToEntity());
        }

        await base.SaveChangesAsync(cancellationToken);
        _savingAuditEntries = false;
    }

    private sealed class PendingAuditEntry
    {
        public string EntityName { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public int? ChangedByUserId { get; set; }

        public Dictionary<string, object?> KeyValues { get; } = new();

        public Dictionary<string, object?> OldValues { get; } = new();

        public Dictionary<string, object?> NewValues { get; } = new();

        public List<PropertyEntry> TemporaryProperties { get; } = new();

        public bool HasChanges =>
            KeyValues.Count > 0 &&
            (ActionType == "DELETE" || ActionType == "CREATE" || OldValues.Count > 0 || NewValues.Count > 0);

        public AppChangeLog ToEntity()
        {
            return new AppChangeLog
            {
                EntityName = EntityName,
                EntityId = string.Join("; ", KeyValues.Select(item => $"{ToSnakeCase(item.Key)}={FormatValue(item.Value)}")),
                ActionType = ActionType,
                OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
                ChangedAt = DateTime.Now,
                ChangedByUserId = ChangedByUserId
            };
        }
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "NULL",
            DateTime dateTime => dateTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
            decimal number => number.ToString("0.00", CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var chars = new List<char> { char.ToUpperInvariant(value[0]) };
        for (var i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]) && value[i - 1] != '_')
            {
                chars.Add('_');
            }

            chars.Add(char.ToUpperInvariant(value[i]));
        }

        return new string(chars.ToArray());
    }
}

internal static class ModelBuilderNamingExtensions
{
    public static void ApplyUpperSnakeCaseNaming(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (entity.GetTableName() is { } tableName)
            {
                entity.SetTableName(ToUpperSnakeCase(tableName));
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToUpperSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToUpperSnakeCase(key.GetName() ?? string.Empty));
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToUpperSnakeCase(index.GetDatabaseName() ?? string.Empty));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(ToUpperSnakeCase(foreignKey.GetConstraintName() ?? string.Empty));
            }
        }
    }

    private static string ToUpperSnakeCase(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        var chars = new List<char>();
        for (var i = 0; i < source.Length; i++)
        {
            var current = source[i];
            var previous = i > 0 ? source[i - 1] : '\0';

            if (i > 0 && char.IsUpper(current) && previous != '_' && !char.IsUpper(previous))
            {
                chars.Add('_');
            }

            chars.Add(char.ToUpperInvariant(current));
        }

        return new string(chars.ToArray());
    }
}
