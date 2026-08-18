namespace PPEInventory.Domain.Entities;

public class Warehouse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public User? UpdatedByUser { get; set; }

    public ICollection<InventoryAdjustment> InventoryAdjustments { get; set; }
    = new List<InventoryAdjustment>();

    public ICollection<InventoryCount> InventoryCounts { get; set; }
    = new List<InventoryCount>();

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; }
    = new List<GoodsReceipt>();

    public ICollection<InventoryBalance> InventoryBalances { get; set; }
        = new List<InventoryBalance>();

    public ICollection<InventoryMovement> InventoryMovements { get; set; }
        = new List<InventoryMovement>();
}