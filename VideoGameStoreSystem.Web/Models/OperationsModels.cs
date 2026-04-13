using System.ComponentModel.DataAnnotations;

namespace VideoGameStoreSystem.Web.Models;

public class Supply
{
    public int SupplyId { get; set; }

    public DateTime SupplyDate { get; set; } = DateTime.Now;

    public int SupplierId { get; set; }

    public decimal TotalAmount { get; set; }

    public Supplier? Supplier { get; set; }

    public ICollection<SupplyItem> Items { get; set; } = new List<SupplyItem>();
}

public class SupplyItem
{
    public int SupplyItemId { get; set; }

    public int SupplyId { get; set; }

    public int ProductId { get; set; }

    [Range(1, 10000)]
    public int Quantity { get; set; }

    [Range(0, 999999)]
    public decimal UnitCost { get; set; }

    public Supply? Supply { get; set; }

    public Product? Product { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Phone, StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    public int? UserId { get; set; }

    public AppUser? User { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

public class Sale
{
    public int SaleId { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.Now;

    public int? CustomerId { get; set; }

    public int SellerUserId { get; set; }

    public decimal TotalAmount { get; set; }

    public Customer? Customer { get; set; }

    public AppUser? SellerUser { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}

public class SaleItem
{
    public int SaleItemId { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    [Range(1, 10000)]
    public int Quantity { get; set; }

    [Range(0, 999999)]
    public decimal UnitPrice { get; set; }

    [Range(0, 999999)]
    public decimal LineTotal { get; set; }

    public Sale? Sale { get; set; }

    public Product? Product { get; set; }
}

public class WriteOff
{
    public int WriteOffId { get; set; }

    public DateTime WriteOffDate { get; set; } = DateTime.Now;

    [Required, StringLength(300)]
    public string Reason { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }

    public AppUser? CreatedByUser { get; set; }

    public ICollection<WriteOffItem> Items { get; set; } = new List<WriteOffItem>();
}

public class WriteOffItem
{
    public int WriteOffItemId { get; set; }

    public int WriteOffId { get; set; }

    public int ProductId { get; set; }

    [Range(1, 10000)]
    public int Quantity { get; set; }

    public WriteOff? WriteOff { get; set; }

    public Product? Product { get; set; }
}
