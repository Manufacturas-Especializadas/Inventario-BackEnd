namespace PPEInventory.Application.Features.PPERequests.Delivery;

public class DeliverPPERequestResultDto
{
    public int PPERequestId { get; set; }

    public string Folio { get; set; } = string.Empty;

    public string EmployeeNumber { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public DateTime DeliveredAt { get; set; }

    public IReadOnlyCollection<DeliveredPPEItemDto> Items { get; set; }
        = Array.Empty<DeliveredPPEItemDto>();
}