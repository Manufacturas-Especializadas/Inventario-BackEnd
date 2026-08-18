using MediatR;

namespace PPEInventory.Application.Features.PPECategories.Queries.GetAll;

public record GetPPECategoriesQuery
    : IRequest<IReadOnlyList<PPECategoryDto>>;