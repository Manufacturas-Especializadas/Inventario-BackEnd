using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPending;

public record GetPendingPPERequestsQuery(
    int? WarehouseId)
    : IRequest<IReadOnlyList<PPERequestDto>>;