using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Reject;

public class RejectPPERequestResultDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public PPERequestStatus Status { get; set; }
    public int RejectedByUserId { get; set; }
    public DateTime RejectedAt { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
