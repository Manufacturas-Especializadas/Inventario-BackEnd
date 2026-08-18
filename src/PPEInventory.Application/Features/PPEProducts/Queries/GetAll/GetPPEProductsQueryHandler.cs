using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPEProducts.Queries.GetAll;

public class GetPPEProductsQueryHandler
    : IRequestHandler<
        GetPPEProductsQuery,
        IReadOnlyList<PPEProductDto>>
{
    private readonly IPPEProductRepository _repository;

    public GetPPEProductsQueryHandler(
        IPPEProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PPEProductDto>> Handle(
        GetPPEProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products =
            await _repository.GetAllAsync(
                cancellationToken);

        return products
            .Select(x => new PPEProductDto
            {
                Id = x.Id,
                Sku = x.Sku,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                Name = x.Name,
                Description = x.Description,

                Size = x.Size,
                Color = x.Color,
                Model = x.Model,
                Specification = x.Specification,

                StockUnit = x.StockUnit,

                MinimumStock = x.MinimumStock,

                MaxQuantityPerRequest =
                    x.MaxQuantityPerRequest,

                ReplacementIntervalDays =
                    x.ReplacementIntervalDays,

                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}