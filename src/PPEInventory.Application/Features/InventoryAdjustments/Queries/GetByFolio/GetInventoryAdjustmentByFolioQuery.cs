using MediatR;

namespace PPEInventory.Application.Features.InventoryAdjustments.Queries.GetByFolio;

public record GetInventoryAdjustmentByFolioQuery(
    string Folio)
    : IRequest<InventoryAdjustmentDto>;