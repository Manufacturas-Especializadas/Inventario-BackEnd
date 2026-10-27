using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests;

public class PPERequestDto
{
    public int Id { get; set; }

    public string Folio { get; set; } = string.Empty;

    public PPERequestStatus Status { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public int? RequestedForOrganizationalUnitId { get; set; }

    public string? RequestedForOrganizationalUnitName { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public int RequestReasonId { get; set; }

    public string RequestReason { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }
    public int? DeliveredByUserId { get; set; }

    public string? DeliveredByUserName { get; set; }

    public DateTime? CancelledAt { get; set; }
    public int? ApprovedByUserId { get; set; }

    public string? ApprovedByUserName { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? CancellationReason { get; set; }

    public IReadOnlyCollection<PPERequestItemDto> Items { get; set; }
        = Array.Empty<PPERequestItemDto>();
}