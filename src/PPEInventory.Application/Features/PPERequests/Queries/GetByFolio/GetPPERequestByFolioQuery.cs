using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetByFolio;

public record GetPPERequestByFolioQuery(
    string Folio)
    : IRequest<PPERequestDto>;