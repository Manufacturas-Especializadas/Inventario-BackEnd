using MediatR;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPendingApproval;

public record GetPendingApprovalPPERequestsQuery(
    int PageNumber = 1,
    int PageSize = PaginationParameters.DefaultPageSize)
    : IRequest<PagedResult<PendingApprovalPPERequestDto>>;
