using MediatR;

namespace PPEInventory.Application.Features
    .InventoryCounts.Commands.Cancel;

public record CancelInventoryCountCommand(
    string Folio,
    string Reason)
    : IRequest<InventoryCountDto>;