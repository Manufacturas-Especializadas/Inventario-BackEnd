namespace PPEInventory.Domain.Entities;

public class PPEProduct
{
    public int Id { get; set; }

    public string Sku { get; private set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public string? Model { get; set; }

    public string? Specification { get; set; }

    public string StockUnit { get; set; } = string.Empty;

    public int MinimumStock { get; set; }

    public int? MaxQuantityPerRequest { get; set; }

    public int? ReplacementIntervalDays { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public PPECategory Category { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;

    public User? UpdatedByUser { get; set; }

    public ICollection<InventoryAdjustmentItem> InventoryAdjustmentItems { get; set; }
    = new List<InventoryAdjustmentItem>();

    public ICollection<InventoryCountItem> InventoryCountItems { get; set; }
    = new List<InventoryCountItem>();

    public ICollection<ProductSupplier> ProductSuppliers { get; set; }
    = new List<ProductSupplier>();

    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    = new List<PurchaseOrderItem>();

    public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    = new List<GoodsReceiptItem>();

    public ICollection<InventoryBalance> InventoryBalances { get; set; }
        = new List<InventoryBalance>();

    public ICollection<InventoryMovement> InventoryMovements { get; set; }
        = new List<InventoryMovement>();
}