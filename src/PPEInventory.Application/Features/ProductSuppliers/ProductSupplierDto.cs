namespace PPEInventory.Application.Features.ProductSuppliers;

public class ProductSupplierDto
{
    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public string? SupplierProductCode { get; set; }

    public string? PackageBarcode { get; set; }

    public string PurchaseUnit { get; set; } = string.Empty;

    public int UnitsPerPackage { get; set; }

    public bool IsPreferred { get; set; }

    public bool IsActive { get; set; }
}