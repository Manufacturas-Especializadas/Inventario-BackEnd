using FluentValidation;

namespace PPEInventory.Application.Features.Employees.Commands.SetStatus;

public class SetEmployeeStatusCommandValidator
    : AbstractValidator<SetEmployeeStatusCommand>
{
    public SetEmployeeStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}