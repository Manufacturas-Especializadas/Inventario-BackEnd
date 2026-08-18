using PPEInventory.Domain.Enums;

namespace PPEInventory.Domain.Entities;

public class PPERequest
{
    public int Id { get; set; }

    public string Folio { get; private set; } = string.Empty;

    public int EmployeeId { get; set; }

    public int WarehouseId { get; set; }

    public int RequestReasonId { get; set; }

    public PPERequestStatus Status { get; set; }

    public string? Notes { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? DeliveredByUserId { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public int? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Employee Employee { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public RequestReason RequestReason { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;

    public User? DeliveredByUser { get; set; }

    public User? CancelledByUser { get; set; }

    public ICollection<PPERequestItem> Items { get; set; }
        = new List<PPERequestItem>();
}