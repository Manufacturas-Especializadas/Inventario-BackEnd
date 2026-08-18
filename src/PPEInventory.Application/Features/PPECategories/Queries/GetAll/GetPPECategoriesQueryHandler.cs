using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPECategories.Queries.GetAll;

public class GetPPECategoriesQueryHandler
    : IRequestHandler<
        GetPPECategoriesQuery,
        IReadOnlyList<PPECategoryDto>>
{
    private readonly IPPECategoryRepository _repository;

    public GetPPECategoriesQueryHandler(
        IPPECategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PPECategoryDto>> Handle(
        GetPPECategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories =
            await _repository.GetAllAsync(
                cancellationToken);

        return categories
            .Select(x => new PPECategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}