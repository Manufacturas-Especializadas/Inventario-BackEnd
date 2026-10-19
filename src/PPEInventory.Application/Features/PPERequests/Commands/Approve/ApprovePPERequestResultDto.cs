using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Approve;

public class ApprovePPERequestResultDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public PPERequestStatus Status { get; set; }
    public int ApprovedByUserId { get; set; }
    public DateTime ApprovedAt { get; set; }
}
