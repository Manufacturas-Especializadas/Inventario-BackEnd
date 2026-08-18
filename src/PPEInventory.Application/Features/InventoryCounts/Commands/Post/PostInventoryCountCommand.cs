using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Post;

public record PostInventoryCountCommand(
    string Folio)
    : IRequest<InventoryCountDto>;