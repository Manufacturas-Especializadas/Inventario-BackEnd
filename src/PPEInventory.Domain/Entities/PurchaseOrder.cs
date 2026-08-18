using PPEInventory.Domain.Enums;

namespace PPEInventory.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }

    public string Folio { get; private set; } = string.Empty;

    public int SupplierId { get; set; }

    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public PurchaseOrderStatus Status { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ConfirmedDeliveryDate { get; set; }

    public DateTime? SupplierConfirmedAt { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public int? CancelledByUserId { get; set; }

    public string? CancellationReason { get; set; }

    public Supplier Supplier { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;

    public User? UpdatedByUser { get; set; }

    public User? CancelledByUser { get; set; }

    public GoodsReceipt? GoodsReceipt { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; }
        = new List<PurchaseOrderItem>();
}