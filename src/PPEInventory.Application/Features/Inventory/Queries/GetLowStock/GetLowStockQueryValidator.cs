using FluentValidation;

namespace PPEInventory.Application.Features.Inventory.Queries.GetLowStock;

public class GetLowStockQueryValidator
    : AbstractValidator<GetLowStockQuery>
{
    public GetLowStockQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0);
    }
}