namespace PPEInventory.Application.Features.InventoryAdjustments;

public class InventoryAdjustmentSummaryDto
{
    public int Id { get; set; }

    public string Folio { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseCode { get; set; } = string.Empty;

    public string WarehouseName { get; set; } = string.Empty;

    public string Reason { get; set; }  = string.Empty;

    public int CreatedByUserId { get; set; }

    public string CreatedByName { get; set; }  = string.Empty;

    public DateTime CreatedAt { get; set; }
}