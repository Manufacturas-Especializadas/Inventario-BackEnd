using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Submit;

public record SubmitInventoryCountCommand(
    string Folio)
    : IRequest<InventoryCountDto>;