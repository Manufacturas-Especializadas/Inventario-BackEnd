using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.CaptureItem;

public record CaptureInventoryCountItemCommand(
    string Folio,
    int PPEProductId,
    int CountedQuantity)
    : IRequest<InventoryCountItemDto>;