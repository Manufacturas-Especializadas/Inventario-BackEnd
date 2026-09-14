using FluentValidation;

namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Cancel;

public class CancelPurchaseOrderCommandValidator
    : AbstractValidator<
        CancelPurchaseOrderCommand>
{
    public CancelPurchaseOrderCommandValidator()
    {
        RuleFor(x => x.Folio)
            .NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}