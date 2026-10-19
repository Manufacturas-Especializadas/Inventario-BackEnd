using MediatR;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPendingApproval;

public class GetPendingApprovalPPERequestsQueryHandler
    : IRequestHandler<GetPendingApprovalPPERequestsQuery, PagedResult<PendingApprovalPPERequestDto>>
{
    private readonly IPPERequestRepository _requestRepository;

    public GetPendingApprovalPPERequestsQueryHandler(IPPERequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public Task<PagedResult<PendingApprovalPPERequestDto>> Handle(
        GetPendingApprovalPPERequestsQuery request,
        CancellationToken cancellationToken)
    {
        return _requestRepository.GetPendingApprovalPageAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
