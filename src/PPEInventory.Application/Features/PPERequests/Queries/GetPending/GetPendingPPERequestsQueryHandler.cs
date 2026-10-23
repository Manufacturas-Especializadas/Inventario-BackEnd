using MediatR;
using PPEInventory.Application.Interfaces;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPending;

public class GetPendingPPERequestsQueryHandler
    : IRequestHandler<
        GetPendingPPERequestsQuery,
        PagedResult<PendingPPERequestDto>>
{
    private readonly IPPERequestRepository _repository;

    public GetPendingPPERequestsQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<PendingPPERequestDto>> Handle(
        GetPendingPPERequestsQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedRequest = request with
        {
            Search = string.IsNullOrWhiteSpace(request.Search)
                ? null
                : request.Search.Trim()
        };

        return _repository.GetPendingAsync(
            normalizedRequest,
            cancellationToken);
    }
}
