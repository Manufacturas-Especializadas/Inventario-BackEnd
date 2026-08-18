using MediatR;
using PPEInventory.Application.Common.Models;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.Inventory.Queries.GetMovements;

public record GetInventoryMovementsQuery(
    int? WarehouseId,
    int? PPEProductId,
    InventoryMovementType? MovementType,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber = 1,
    int PageSize = PaginationParameters.DefaultPageSize)
    : IRequest<PagedResult<InventoryMovementDto>>;