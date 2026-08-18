using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPendingByEmployee;

public class GetPendingPPERequestsByEmployeeQueryHandler
    : IRequestHandler<
        GetPendingPPERequestsByEmployeeQuery,
        IReadOnlyList<PPERequestDto>>
{
    private readonly IPPERequestRepository _repository;

    public GetPendingPPERequestsByEmployeeQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PPERequestDto>> Handle(
        GetPendingPPERequestsByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var requests =
            await _repository
                .GetPendingByEmployeeNumberAsync(
                    employeeNumber,
                    cancellationToken);

        return requests
            .Select(x => new PPERequestDto
            {
                Id = x.Id,
                Folio = x.Folio,
                Status = x.Status,

                EmployeeId =
                    x.EmployeeId,

                EmployeeNumber =
                    x.Employee.EmployeeNumber,

                EmployeeName =
                    x.Employee.Name,

                WarehouseId =
                    x.WarehouseId,

                WarehouseName =
                    x.Warehouse.Name,

                RequestReasonId =
                    x.RequestReasonId,

                RequestReason =
                    x.RequestReason.Name,

                Notes =
                    x.Notes,

                CreatedAt =
                    x.CreatedAt,

                DeliveredAt =
                    x.DeliveredAt,

                CancelledAt =
                    x.CancelledAt,

                CancellationReason =
                    x.CancellationReason,

                Items =
                    x.Items
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
            })
            .ToArray();
    }
}