namespace PPEInventory.Domain.Entities;

public class GoodsReceipt
{
    public int Id { get; set; }

    public string Folio { get; private set; } = string.Empty;

    public int PurchaseOrderId { get; set; }

    public int WarehouseId { get; set; }

    public DateTime ReceivedAt { get; set; }

    public int ReceivedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public User ReceivedByUser { get; set; } = null!;

    public ICollection<GoodsReceiptItem> Items { get; set; }
        = new List<GoodsReceiptItem>();
}