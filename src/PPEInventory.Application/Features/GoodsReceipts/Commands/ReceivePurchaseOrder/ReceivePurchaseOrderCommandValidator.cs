using FluentValidation;

namespace PPEInventory.Application.Features.GoodsReceipts.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandValidator
    : AbstractValidator<ReceivePurchaseOrderCommand>
{
    public ReceivePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderFolio)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0);

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}