using FluentValidation;

namespace PPEInventory.Application.Features.InventoryAdjustments.Commands.Create;

public class CreateInventoryAdjustmentCommandValidator
    : AbstractValidator<CreateInventoryAdjustmentCommand>
{
    public CreateInventoryAdjustmentCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage(
                "Inventory adjustment must contain at least one item.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.PPEProductId)
                    .GreaterThan(0);

                item.RuleFor(x => x.QuantityAdjustment)
                    .NotEqual(0)
                    .WithMessage(
                        "Adjustment quantity cannot be zero.");
            });

        RuleFor(x => x.Items)
            .Must(items =>
                items
                    .Select(x => x.PPEProductId)
                    .Distinct()
                    .Count() == items.Count)
            .WithMessage(
                "The same PPE product cannot appear more than once.");
    }
}