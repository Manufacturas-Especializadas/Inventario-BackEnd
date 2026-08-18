namespace PPEInventory.Domain.Entities;

public class GoodsReceiptItem
{
    public int Id { get; set; }

    public int GoodsReceiptId { get; set; }

    public int PurchaseOrderItemId { get; set; }

    public int PPEProductId { get; set; }

    public int ReceivedQuantity { get; set; }

    public GoodsReceipt GoodsReceipt { get; set; } = null!;

    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;
}