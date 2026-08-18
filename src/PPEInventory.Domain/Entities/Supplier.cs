namespace PPEInventory.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ContactName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public User? UpdatedByUser { get; set; }

    public ICollection<ProductSupplier> ProductSuppliers { get; set; }
        = new List<ProductSupplier>();

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    = new List<PurchaseOrder>();
}