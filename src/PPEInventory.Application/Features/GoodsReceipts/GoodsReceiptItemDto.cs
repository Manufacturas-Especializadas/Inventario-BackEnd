namespace PPEInventory.Application.Features.GoodsReceipts;

public class GoodsReceiptItemDto
{
    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string PurchaseUnit { get; set; } = string.Empty;

    public int UnitsPerPackage { get; set; }

    public int OrderedPurchaseQuantity { get; set; }

    public int ReceivedQuantity { get; set; }
}