using PPEInventory.Domain.Enums;

namespace PPEInventory.Domain.Entities;

public class InventoryCount
{
    public int Id { get; set; }

    public string Folio { get; private set; } = string.Empty;

    public int WarehouseId { get; set; }

    public InventoryCountStatus Status { get; set; }

    public string? Notes { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public int? PostedByUserId { get; set; }

    public DateTime? PostedAt { get; set; }

    public int? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;

    public User? SubmittedByUser { get; set; }

    public User? PostedByUser { get; set; }

    public User? CancelledByUser { get; set; }

    public ICollection<InventoryCountItem> Items { get; set; }
        = new List<InventoryCountItem>();
}