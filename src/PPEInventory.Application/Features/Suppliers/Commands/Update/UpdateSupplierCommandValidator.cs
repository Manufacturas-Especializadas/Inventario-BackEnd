using FluentValidation;

namespace PPEInventory.Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommandValidator
    : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ContactName)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(
                x =>
                    !string.IsNullOrWhiteSpace(
                        x.Email
                    )
            );

        RuleFor(x => x.Phone)
            .MaximumLength(30);
    }
}