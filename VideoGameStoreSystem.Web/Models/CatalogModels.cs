using System.ComponentModel.DataAnnotations;

namespace VideoGameStoreSystem.Web.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int ProductId { get; set; }

    [Required, StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    [Required, StringLength(40)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, 999999)]
    public decimal Price { get; set; }

    [Range(0, 999999)]
    public decimal CostPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public Category? Category { get; set; }

    public ICollection<SupplyItem> SupplyItems { get; set; } = new List<SupplyItem>();

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public ICollection<WriteOffItem> WriteOffItems { get; set; } = new List<WriteOffItem>();
}

public class Supplier
{
    public int SupplierId { get; set; }

    [Required, StringLength(150)]
    public string SupplierName { get; set; } = string.Empty;

    [Phone, StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    public ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
