using PPEInventory.Application.Features.RequestReasons;
using PPEInventory.Application.Features.Users;

namespace PPEInventory.Application.Features.PPERequests;

public class PPERequestFormOptionsDto
{
    public IReadOnlyList<ManagedOrganizationalUnitDto> ManagedOrganizationalUnits { get; set; }
        = Array.Empty<ManagedOrganizationalUnitDto>();

    public IReadOnlyList<PPERequestWarehouseDto> Warehouses { get; set; }
        = Array.Empty<PPERequestWarehouseDto>();

    public IReadOnlyList<RequestReasonDto> RequestReasons { get; set; }
        = Array.Empty<RequestReasonDto>();
}
