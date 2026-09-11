using FluentValidation;

namespace PPEInventory.Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommandValidator
    : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .WithMessage(
                "Employee number is required.")
            .MaximumLength(20);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(
                "Employee name is required.")
            .MaximumLength(150);

        RuleFor(
                x => x.OrganizationalUnitId)
            .GreaterThan(0);
    }
}