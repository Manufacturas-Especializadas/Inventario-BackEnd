namespace PPEInventory.Domain.Entities;

public class ProductSupplier
{
    public int PPEProductId { get; set; }

    public int SupplierId { get; set; }

    public string? SupplierProductCode { get; set; }

    public string? PackageBarcode { get; set; }

    public string PurchaseUnit { get; set; } = string.Empty;

    public int UnitsPerPackage { get; set; }

    public bool IsPreferred { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public PPEProduct PPEProduct { get; set; } = null!;

    public Supplier Supplier { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;
}