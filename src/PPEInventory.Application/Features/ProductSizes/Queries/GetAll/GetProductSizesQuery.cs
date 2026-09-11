using MediatR;

namespace PPEInventory.Application.Features.ProductSizes.Queries.GetAll;

public record GetProductSizesQuery() : IRequest<IReadOnlyList<ProductSizeDto>>;