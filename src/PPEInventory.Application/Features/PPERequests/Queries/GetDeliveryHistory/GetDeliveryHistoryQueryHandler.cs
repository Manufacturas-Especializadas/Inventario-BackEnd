using MediatR;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetDeliveryHistory;

public class GetDeliveryHistoryQueryHandler
    : IRequestHandler<
        GetDeliveryHistoryQuery,
        PagedResult<
            DeliveryHistoryPPERequestDto>>
{
    private readonly
        IPPERequestRepository _repository;

    public GetDeliveryHistoryQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public Task<
        PagedResult<
            DeliveryHistoryPPERequestDto>>
        Handle(
            GetDeliveryHistoryQuery request,
            CancellationToken cancellationToken)
    {
        var normalizedRequest =
            request with
            {
                Search =
                    string.IsNullOrWhiteSpace(
                        request.Search)
                        ? null
                        : request.Search.Trim()
            };

        return _repository
            .GetDeliveryHistoryAsync(
                normalizedRequest,
                cancellationToken);
    }
}