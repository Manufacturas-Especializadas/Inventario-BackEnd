using PPEInventory.Application.Features.RequestReasons;

namespace PPEInventory.Application.Features.PPERequests;

public class PPERequestFormOptionsDto
{
    public IReadOnlyList<PPERequestWarehouseDto> Warehouses { get; set; }
        = Array.Empty<PPERequestWarehouseDto>();

    public IReadOnlyList<RequestReasonDto> RequestReasons { get; set; }
        = Array.Empty<RequestReasonDto>();
}
