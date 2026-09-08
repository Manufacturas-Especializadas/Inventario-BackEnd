using FluentValidation;

namespace PPEInventory.Application.Features.OrganizationalUnits.Commands.Create;

public class CreateOrganizationalUnitCommandValidator
    : AbstractValidator<CreateOrganizationalUnitCommand>
{
    public CreateOrganizationalUnitCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.ParentId)
            .GreaterThan(0)
            .When(x => x.ParentId.HasValue);
    }
}