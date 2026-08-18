using FluentValidation;

namespace PPEInventory.Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommandValidator
    : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .WithMessage("Employee number is required.")
            .MaximumLength(20);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Employee name is required.")
            .MaximumLength(150);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);

        RuleFor(x => x.LineId)
            .GreaterThan(0)
            .When(x => x.LineId.HasValue);
    }
}