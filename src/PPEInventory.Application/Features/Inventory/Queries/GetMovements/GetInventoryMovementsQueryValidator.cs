using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.Inventory.Queries.GetMovements;

public class GetInventoryMovementsQueryValidator
    : AbstractValidator<GetInventoryMovementsQuery>
{
    public GetInventoryMovementsQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .When(x => x.WarehouseId.HasValue);

        RuleFor(x => x.PPEProductId)
            .GreaterThan(0)
            .When(x => x.PPEProductId.HasValue);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(
                1,
                PaginationParameters.MaxPageSize);

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x =>
                x.DateFrom.HasValue &&
                x.DateTo.HasValue);
    }
}