using MediatR;

namespace PPEInventory.Application.Features.InventoryAdjustments.Queries.GetList;

public record GetInventoryAdjustmentsQuery(
    int? WarehouseId,
    DateOnly? DateFrom,
    DateOnly? DateTo)
    : IRequest<
        IReadOnlyList<
            InventoryAdjustmentSummaryDto>>;