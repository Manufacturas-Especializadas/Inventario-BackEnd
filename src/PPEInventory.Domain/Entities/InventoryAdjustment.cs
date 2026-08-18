namespace PPEInventory.Domain.Entities;

public class InventoryAdjustment
{
    public int Id { get; set; }

    public string Folio { get; private set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;

    public ICollection<InventoryAdjustmentItem> Items { get; set; }
        = new List<InventoryAdjustmentItem>();
}