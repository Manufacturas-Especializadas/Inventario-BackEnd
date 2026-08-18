using FluentValidation;

namespace PPEInventory.Application.Features.ProductionLines.Commands.Create;

public class CreateProductionLineCommandValidator
    : AbstractValidator<CreateProductionLineCommand>
{
    public CreateProductionLineCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Production line name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}