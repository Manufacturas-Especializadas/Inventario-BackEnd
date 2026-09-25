using MediatR;
using PPEInventory.Application.Common.Models;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetAll;

public record GetPurchaseOrdersQuery(
    PurchaseOrderStatus? Status = null,
    int PageNumber = 1,
    int PageSize = PaginationParameters.DefaultPageSize)
    : IRequest<PagedResult<PurchaseOrderDto>>;