using FluentValidation;

namespace PPEInventory.Application.Features
    .InventoryCounts.Commands.Cancel;

public class CancelInventoryCountCommandValidator
    : AbstractValidator<
        CancelInventoryCountCommand>
{
    public CancelInventoryCountCommandValidator()
    {
        RuleFor(x => x.Folio)
            .NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}