using PPEInventory.Domain.Enums;

namespace PPEInventory.Domain.Entities;

public class InventoryMovement
{
    public long Id { get; set; }

    public int WarehouseId { get; set; }

    public int PPEProductId { get; set; }

    public InventoryMovementType MovementType { get; set; }

    public int Quantity { get; set; }

    public InventoryReferenceType ReferenceType { get; set; }

    public int ReferenceId { get; set; }

    public decimal? UnitCost { get; set; }

    public string? Reason { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;
}