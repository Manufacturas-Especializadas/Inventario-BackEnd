using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryCounts;

public class InventoryCountDto
{
    public int Id { get; set; }

    public string Folio { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseCode { get; set; } = string.Empty;

    public string WarehouseName { get; set; } = string.Empty;

    public InventoryCountStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? PostedAt { get; set; }

    public IReadOnlyCollection<InventoryCountItemDto> Items { get; set; }
        = Array.Empty<InventoryCountItemDto>();
}