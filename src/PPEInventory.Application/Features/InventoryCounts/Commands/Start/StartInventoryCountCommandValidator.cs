using FluentValidation;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.Start;

public class StartInventoryCountCommandValidator
    : AbstractValidator<StartInventoryCountCommand>
{
    public StartInventoryCountCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0);

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}