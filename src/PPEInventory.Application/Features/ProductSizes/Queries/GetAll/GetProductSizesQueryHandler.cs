using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductSizes.Queries.GetAll;

public class GetProductSizesQueryHandler
    : IRequestHandler<
        GetProductSizesQuery,
        IReadOnlyList<ProductSizeDto>>
{
    private readonly IProductSizeRepository
        _repository;

    public GetProductSizesQueryHandler(
        IProductSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<ProductSizeDto>>
        Handle(
            GetProductSizesQuery request,
            CancellationToken cancellationToken)
    {
        var sizes =
            await _repository.GetAllAsync(
                cancellationToken);

        return sizes
            .Select(x =>
                new ProductSizeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
            .ToArray();
    }
}