using FluentValidation;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetRequestableProducts;

public class GetRequestablePPEProductsQueryValidator
    : AbstractValidator<GetRequestablePPEProductsQuery>
{
    public GetRequestablePPEProductsQueryValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0);
    }
}
