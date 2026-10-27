using FluentValidation;

namespace PPEInventory.Application.Features.Users.Commands.SetManagedOrganizationalUnits;

public class SetUserManagedOrganizationalUnitsCommandValidator
    : AbstractValidator<SetUserManagedOrganizationalUnitsCommand>
{
    public SetUserManagedOrganizationalUnitsCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.OrganizationalUnitIds).NotNull();
        RuleForEach(x => x.OrganizationalUnitIds).GreaterThan(0);
        RuleFor(x => x.OrganizationalUnitIds)
            .Must(ids => ids is null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Organizational unit IDs must not contain duplicates.");
    }
}
