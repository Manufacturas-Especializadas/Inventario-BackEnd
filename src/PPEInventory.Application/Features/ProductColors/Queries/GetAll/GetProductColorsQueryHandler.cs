using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductColors.Queries.GetAll;

public class GetProductColorsQueryHandler
    : IRequestHandler<
        GetProductColorsQuery,
        IReadOnlyList<ProductColorDto>>
{
    private readonly IProductColorRepository
        _repository;

    public GetProductColorsQueryHandler(
        IProductColorRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<ProductColorDto>>
        Handle(
            GetProductColorsQuery request,
            CancellationToken cancellationToken)
    {
        var colors =
            await _repository.GetAllAsync(
                cancellationToken);

        return colors
            .Select(x =>
                new ProductColorDto
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