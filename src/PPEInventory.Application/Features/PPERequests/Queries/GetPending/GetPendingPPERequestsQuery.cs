using MediatR;

using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetPending;

/// <summary>
/// Delivery work queue: only Pending requests, the state accepted by DeliverPPERequest.
/// Exceptional requests enter this state only after approval reserves their inventory.
/// Date filters are inclusive UTC calendar dates of creation.
/// </summary>
public record GetPendingPPERequestsQuery(
    int? WarehouseId = null,
    string? Search = null,
    int? RequestedForOrganizationalUnitId = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    int PageNumber = 1,
    int PageSize = PaginationParameters.DefaultPageSize)
    : IRequest<PagedResult<PendingPPERequestDto>>;
