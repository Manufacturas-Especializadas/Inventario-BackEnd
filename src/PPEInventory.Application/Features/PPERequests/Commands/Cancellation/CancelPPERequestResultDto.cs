namespace PPEInventory.Application.Features.PPERequests.Cancellation;

public class CancelPPERequestResultDto
{
    public int PPERequestId { get; set; }

    public string Folio { get; set; } = string.Empty;

    public string EmployeeNumber { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public DateTime CancelledAt { get; set; }

    public string CancellationReason { get; set; } = string.Empty;

    public IReadOnlyCollection<CancelledPPEItemDto> Items { get; set; }
        = Array.Empty<CancelledPPEItemDto>();
}