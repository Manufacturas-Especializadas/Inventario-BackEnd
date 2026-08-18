using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Start;

public record StartInventoryCountCommand(
    int WarehouseId,
    string? Notes)
    : IRequest<InventoryCountDto>;