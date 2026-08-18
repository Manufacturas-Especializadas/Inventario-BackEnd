using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.PPERequests;

public static class PPERequestMappings
{
    public static PPERequestDto ToDto(
        this PPERequest request)
    {
        return new PPERequestDto
        {
            Id = request.Id,
            Folio = request.Folio,
            Status = request.Status,

            EmployeeId =
                request.EmployeeId,

            EmployeeNumber =
                request.Employee.EmployeeNumber,

            EmployeeName =
                request.Employee.Name,

            WarehouseId =
                request.WarehouseId,

            WarehouseName =
                request.Warehouse.Name,

            RequestReasonId =
                request.RequestReasonId,

            RequestReason =
                request.RequestReason.Name,

            Notes =
                request.Notes,

            CreatedAt =
                request.CreatedAt,

            DeliveredAt =
                request.DeliveredAt,

            CancelledAt =
                request.CancelledAt,

            CancellationReason =
                request.CancellationReason,

            Items =
                request.Items
                    .Select(item =>
                        new PPERequestItemDto
                        {
                            PPEProductId =
                                item.PPEProductId,

                            Sku =
                                item.PPEProduct.Sku,

                            ProductName =
                                item.PPEProduct.Name,

                            Quantity =
                                item.Quantity,

                            ReplacementIntervalDays =
                                item.PPEProduct
                                    .ReplacementIntervalDays
                        })
                    .ToArray()
        };
    }
}