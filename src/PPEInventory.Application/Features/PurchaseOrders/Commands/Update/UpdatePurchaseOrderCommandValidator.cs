using FluentValidation;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features
    .PurchaseOrders.Commands.Update;

public class UpdatePurchaseOrderCommandValidator
    : AbstractValidator<
        UpdatePurchaseOrderCommand>
{
    public UpdatePurchaseOrderCommandValidator(
        IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Folio)
            .NotEmpty();

        RuleFor(x => x.SupplierId)
            .GreaterThan(0);

        RuleFor(x => x.PurchaseOrderNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ConfirmedDeliveryDate)
            .Must(date =>
                date.Date >=
                dateTimeProvider.UtcNow.Date)
            .WithMessage(
                "Confirmed delivery date cannot be in the past.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Za-z]{3}$");

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        RuleFor(x => x.Items)
            .NotEmpty();

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x =>
                        x.PPEProductId)
                    .GreaterThan(0);

                item.RuleFor(x =>
                        x.OrderedPurchaseQuantity)
                    .GreaterThan(0);

                item.RuleFor(x =>
                        x.PurchaseUnitCost)
                    .GreaterThanOrEqualTo(0)
                    .When(x =>
                        x.PurchaseUnitCost
                            .HasValue);
            });

        RuleFor(x => x.Items)
            .Must(items =>
                items
                    .Select(x =>
                        x.PPEProductId)
                    .Distinct()
                    .Count() ==
                items.Count)
            .WithMessage(
                "The same PPE product cannot appear more than once.");
    }
}