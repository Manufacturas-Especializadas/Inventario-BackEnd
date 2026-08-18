using FluentValidation;

namespace PPEInventory.Application.Features.InventoryCounts.Commands.CaptureItem;

public class CaptureInventoryCountItemCommandValidator
    : AbstractValidator<CaptureInventoryCountItemCommand>
{
    public CaptureInventoryCountItemCommandValidator()
    {
        RuleFor(x => x.Folio)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.PPEProductId)
            .GreaterThan(0);

        RuleFor(x => x.CountedQuantity)
            .GreaterThanOrEqualTo(0);
    }
}