using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetByFolio;

public class GetPPERequestByFolioQueryHandler
    : IRequestHandler<
        GetPPERequestByFolioQuery,
        PPERequestDto>
{
    private readonly IPPERequestRepository _repository;

    public GetPPERequestByFolioQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<PPERequestDto> Handle(
        GetPPERequestByFolioQuery request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.Folio.Trim().ToUpperInvariant();

        var ppeRequest =
            await _repository.GetByFolioAsync(
                folio,
                cancellationToken);

        if (ppeRequest is null)
        {
            throw new NotFoundException(
                $"PPE request '{folio}' was not found.");
        }

        return new PPERequestDto
        {
            Id = ppeRequest.Id,
            Folio = ppeRequest.Folio,
            Status = ppeRequest.Status,

            EmployeeId =
                ppeRequest.EmployeeId,

            EmployeeNumber =
                ppeRequest.Employee.EmployeeNumber,

            EmployeeName =
                ppeRequest.Employee.Name,

            WarehouseId =
                ppeRequest.WarehouseId,

            WarehouseName =
                ppeRequest.Warehouse.Name,

            RequestReasonId =
                ppeRequest.RequestReasonId,

            RequestReason =
                ppeRequest.RequestReason.Name,

            Notes =
                ppeRequest.Notes,

            CreatedAt =
                ppeRequest.CreatedAt,

            DeliveredAt = 
                ppeRequest.DeliveredAt,

            CancelledAt =
                ppeRequest.CancelledAt,

            CancellationReason =
                ppeRequest.CancellationReason,

            Items =
                ppeRequest.Items
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