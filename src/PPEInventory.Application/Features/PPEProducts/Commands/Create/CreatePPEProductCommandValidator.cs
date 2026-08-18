using FluentValidation;

namespace PPEInventory.Application.Features.PPEProducts.Commands.Create;

public class CreatePPEProductCommandValidator
    : AbstractValidator<CreatePPEProductCommand>
{
    public CreatePPEProductCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Size)
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .MaximumLength(50);

        RuleFor(x => x.Model)
            .MaximumLength(100);

        RuleFor(x => x.Specification)
            .MaximumLength(250);

        RuleFor(x => x.StockUnit)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxQuantityPerRequest)
            .GreaterThan(0)
            .When(x => x.MaxQuantityPerRequest.HasValue);

        RuleFor(x => x.ReplacementIntervalDays)
            .GreaterThan(0)
            .When(x => x.ReplacementIntervalDays.HasValue);
    }
}