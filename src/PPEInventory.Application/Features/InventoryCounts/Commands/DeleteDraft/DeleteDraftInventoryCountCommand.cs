using MediatR;

namespace PPEInventory.Application.Features
    .InventoryCounts.Commands.DeleteDraft;

public record DeleteDraftInventoryCountCommand(
    string Folio)
    : IRequest;
