namespace PPEInventory.Application.Features.PurchaseOrders;

public class PurchaseOrderItemDto
{
    public int Id { get; set; }

    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string? SupplierProductCode { get; set; }

    public string PurchaseUnit { get; set; } = string.Empty;

    public int UnitsPerPackage { get; set; }

    public int OrderedPurchaseQuantity { get; set; }

    public int OrderedStockQuantity =>
        OrderedPurchaseQuantity * UnitsPerPackage;

    public decimal? PurchaseUnitCost { get; set; }

    public decimal? LineTotal =>
        PurchaseUnitCost.HasValue
            ? PurchaseUnitCost.Value *
              OrderedPurchaseQuantity
            : null;
}