using FluentValidation;

namespace PPEInventory.Application.Features.WarehouseProducts.Commands.BulkAssign;

public class BulkAssignWarehouseProductsCommandValidator
    : AbstractValidator<BulkAssignWarehouseProductsCommand>
{
    public BulkAssignWarehouseProductsCommandValidator()
    {
        RuleFor(x => x.WarehouseIds)
            .NotEmpty();

        RuleFor(x => x.PPEProductIds)
            .NotEmpty();

        RuleForEach(x => x.WarehouseIds)
            .GreaterThan(0);

        RuleForEach(x => x.PPEProductIds)
            .GreaterThan(0);
    }
}