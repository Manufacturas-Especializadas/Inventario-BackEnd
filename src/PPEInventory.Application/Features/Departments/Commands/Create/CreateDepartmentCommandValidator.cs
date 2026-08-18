using FluentValidation;

namespace PPEInventory.Application.Features.Departments.Commands.Create;

public class CreateDepartmentCommandValidator
    : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}