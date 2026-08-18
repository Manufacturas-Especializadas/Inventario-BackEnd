using MediatR;

namespace PPEInventory.Application.Features.InventoryAdjustments.Commands.Create;

public record CreateInventoryAdjustmentCommand(
    int WarehouseId,
    string Reason,
    IReadOnlyCollection<CreateInventoryAdjustmentItemRequest> Items)
    : IRequest<InventoryAdjustmentDto>;