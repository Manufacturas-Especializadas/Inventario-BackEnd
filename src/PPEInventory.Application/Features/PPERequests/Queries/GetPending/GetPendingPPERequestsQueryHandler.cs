using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPending;

public class GetPendingPPERequestsQueryHandler
    : IRequestHandler<
        GetPendingPPERequestsQuery,
        IReadOnlyList<PPERequestDto>>
{
    private readonly IPPERequestRepository _repository;

    public GetPendingPPERequestsQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PPERequestDto>> Handle(
        GetPendingPPERequestsQuery request,
        CancellationToken cancellationToken)
    {
        var requests =
            await _repository.GetPendingAsync(
                request.WarehouseId,
                cancellationToken);

        return requests
            .Select(x => x.ToDto())
            .ToArray();
    }
}