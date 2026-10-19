using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetRequestableProducts;

public record GetRequestablePPEProductsQuery(int WarehouseId)
    : IRequest<IReadOnlyList<RequestablePPEProductDto>>;
