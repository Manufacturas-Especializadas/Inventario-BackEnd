using FluentValidation;

namespace PPEInventory.Application.Features.PurchaseOrders.Commands.Create;

public class CreatePurchaseOrderItemRequestValidator
    : AbstractValidator<CreatePurchaseOrderItemRequest>
{
    public CreatePurchaseOrderItemRequestValidator()
    {
        RuleFor(x => x.PPEProductId)
            .GreaterThan(0);

        RuleFor(x => x.OrderedPurchaseQuantity)
            .GreaterThan(0);

        RuleFor(x => x.PurchaseUnitCost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PurchaseUnitCost.HasValue);
    }
}