using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.Inventory;

public class InventoryMovementDto
{
    public long Id { get; set; }

    public int WarehouseId { get; set; }

    public string WarehouseCode { get; set; } = string.Empty;

    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public InventoryMovementType MovementType { get; set; }

    public int Quantity { get; set; }

    public InventoryReferenceType ReferenceType { get; set; }

    public int ReferenceId { get; set; }

    public decimal? UnitCost { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }
}