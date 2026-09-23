using FluentValidation;

namespace PPEInventory.Application.Features.InventoryAdjustments.Queries.GetList;

public class GetInventoryAdjustmentsQueryValidator
    : AbstractValidator<
        GetInventoryAdjustmentsQuery>
{
    public GetInventoryAdjustmentsQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .When(
                x =>
                    x.WarehouseId.HasValue);

        RuleFor(x => x)
            .Must(
                x =>
                    !x.DateFrom.HasValue ||
                    !x.DateTo.HasValue ||
                    x.DateFrom.Value <=
                    x.DateTo.Value)
            .WithMessage(
                "DateFrom cannot be greater than DateTo.");
    }
}