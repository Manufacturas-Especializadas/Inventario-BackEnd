using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests;

public class PendingApprovalPPERequestDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public PPERequestStatus Status { get; set; }
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
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyCollection<PendingApprovalPPERequestItemDto> Items { get; set; }
        = Array.Empty<PendingApprovalPPERequestItemDto>();
}
