using MediatR;

namespace PPEInventory.Application.Features.ProductColors.Queries.GetAll;

public record GetProductColorsQuery()
    : IRequest<IReadOnlyList<ProductColorDto>>;