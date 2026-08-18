using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Queries.GetByFolio;

public record GetInventoryCountByFolioQuery(
    string Folio)
    : IRequest<InventoryCountDto>;