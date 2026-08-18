using FluentValidation;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PurchaseOrders.Commands.Create;

public class CreatePurchaseOrderCommandValidator
    : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator(
        IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.SupplierId)
            .GreaterThan(0);

        RuleFor(x => x.PurchaseOrderNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ConfirmedDeliveryDate)
            .Must(date =>
                date.Date >= dateTimeProvider.UtcNow.Date)
            .WithMessage(
                "Confirmed delivery date cannot be in the past.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Za-z]{3}$")
            .WithMessage(
                "Currency code must contain exactly three letters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage(
                "Purchase order must contain at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(
                new CreatePurchaseOrderItemRequestValidator());

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