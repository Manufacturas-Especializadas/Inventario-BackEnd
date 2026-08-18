using FluentValidation;

namespace PPEInventory.Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommandValidator
    : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ContactName)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(30);
    }
}