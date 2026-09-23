using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.InventoryAdjustments.Queries.GetList;

public class GetInventoryAdjustmentsQueryHandler
    : IRequestHandler<
        GetInventoryAdjustmentsQuery,
        IReadOnlyList<
            InventoryAdjustmentSummaryDto>>
{
    private readonly
        IInventoryAdjustmentRepository
        _repository;

    public GetInventoryAdjustmentsQueryHandler(
        IInventoryAdjustmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<
            InventoryAdjustmentSummaryDto>>
        Handle(
            GetInventoryAdjustmentsQuery request,
            CancellationToken cancellationToken)
    {
        DateTime? createdFrom =
            request.DateFrom.HasValue
                ? DateTime.SpecifyKind(
                    request.DateFrom.Value
                        .ToDateTime(
                            TimeOnly.MinValue),
                    DateTimeKind.Utc)
                : null;

        DateTime? createdToExclusive =
            request.DateTo.HasValue
                ? DateTime.SpecifyKind(
                    request.DateTo.Value
                        .AddDays(1)
                        .ToDateTime(
                            TimeOnly.MinValue),
                    DateTimeKind.Utc)
                : null;

        var adjustments =
            await _repository
                .GetFilteredAsync(
                    request.WarehouseId,
                    createdFrom,
                    createdToExclusive,
                    cancellationToken);

        return adjustments
            .Select(
                adjustment =>
                    new InventoryAdjustmentSummaryDto
                    {
                        Id =
                            adjustment.Id,

                        Folio =
                            adjustment.Folio,

                        WarehouseId =
                            adjustment.WarehouseId,

                        WarehouseCode =
                            adjustment.Warehouse.Code,

                        WarehouseName =
                            adjustment.Warehouse.Name,

                        Reason =
                            adjustment.Reason,

                        CreatedByUserId =
                            adjustment.CreatedByUserId,

                        CreatedByName =
                            adjustment
                                .CreatedByUser
                                .Employee
                                .Name,

                        CreatedAt =
                            adjustment.CreatedAt,
                    })
            .ToArray();
    }
}