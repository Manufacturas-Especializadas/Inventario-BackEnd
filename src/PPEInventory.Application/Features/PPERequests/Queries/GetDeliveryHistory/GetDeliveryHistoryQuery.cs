using MediatR;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetDeliveryHistory;

public record GetDeliveryHistoryQuery(
    DateOnly DeliveredFrom,
    DateOnly DeliveredTo,
    string? EmployeeNumber = null,
    string? Search = null,
    int? WarehouseId = null,
    int? RequestedForOrganizationalUnitId = null,
    int PageNumber = 1,
    int PageSize =
        PaginationParameters.DefaultPageSize)
    : IRequest<
        PagedResult<DeliveryHistoryPPERequestDto>>;