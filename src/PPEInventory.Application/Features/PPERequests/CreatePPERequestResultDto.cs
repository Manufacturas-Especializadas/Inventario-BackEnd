namespace PPEInventory.Application.Features.PPERequests;

public class CreatePPERequestResultDto
{
    public PPERequestDto Request { get; set; } = null!;

    public IReadOnlyCollection<PPERequestWarningDto> Warnings { get; set; }
        = Array.Empty<PPERequestWarningDto>();
}