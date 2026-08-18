using FluentValidation;

namespace PPEInventory.Application.Features.PPERequests.Commands.Create;

public class CreatePPERequestCommandValidator
    : AbstractValidator<CreatePPERequestCommand>
{
    public CreatePPERequestCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0);

        RuleFor(x => x.RequestReasonId)
            .GreaterThan(0);

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage(
                "PPE request must contain at least one item.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.PPEProductId)
                    .GreaterThan(0);

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0);
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