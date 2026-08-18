namespace PPEInventory.Domain.Entities;

public class PurchaseOrderItem
{
    public int Id { get; set; }

    public int PurchaseOrderId { get; set; }

    public int PPEProductId { get; set; }

    public string? SupplierProductCode { get; set; }

    public string PurchaseUnit { get; set; } = string.Empty;

    public int UnitsPerPackage { get; set; }

    public int OrderedPurchaseQuantity { get; set; }

    public decimal? PurchaseUnitCost { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;
}