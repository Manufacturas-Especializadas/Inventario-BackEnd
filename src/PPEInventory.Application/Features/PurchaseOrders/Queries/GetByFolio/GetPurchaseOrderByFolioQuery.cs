using MediatR;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetByFolio;

public record GetPurchaseOrderByFolioQuery(
    string Folio)
    : IRequest<PurchaseOrderDto>;