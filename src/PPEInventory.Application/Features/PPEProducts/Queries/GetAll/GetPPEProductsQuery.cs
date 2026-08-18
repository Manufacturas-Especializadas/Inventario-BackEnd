using MediatR;

namespace PPEInventory.Application.Features.PPEProducts.Queries.GetAll;

public record GetPPEProductsQuery: IRequest<IReadOnlyList<PPEProductDto>>;