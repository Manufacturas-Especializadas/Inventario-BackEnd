using FluentValidation;

namespace PPEInventory.Application.Features.ProductSuppliers.Commands.Create;

public class CreateProductSupplierCommandValidator
    : AbstractValidator<CreateProductSupplierCommand>
{
    public CreateProductSupplierCommandValidator()
    {
        RuleFor(x => x.PPEProductId)
            .GreaterThan(0);

        RuleFor(x => x.SupplierId)
            .GreaterThan(0);

        RuleFor(x => x.SupplierProductCode)
            .MaximumLength(100);

        RuleFor(x => x.PackageBarcode)
            .MaximumLength(100);

        RuleFor(x => x.PurchaseUnit)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.UnitsPerPackage)
            .GreaterThan(0);
    }
}