using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests;

/// <summary>Delivery queue row; item details are loaded by folio on selection.</summary>
public class PendingPPERequestDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int? RequestedForOrganizationalUnitId { get; set; }
    public string? RequestedForOrganizationalUnitName { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int RequestReasonId { get; set; }
    public string RequestReason { get; set; } = string.Empty;
    public string RequestReasonCode { get; set; } = string.Empty;

    /// <summary>Number of product lines, not the sum of requested quantities.</summary>
    public int ItemCount { get; set; }

    public PPERequestStatus Status { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
