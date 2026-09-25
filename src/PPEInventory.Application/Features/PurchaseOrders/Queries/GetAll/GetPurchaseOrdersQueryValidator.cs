using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PurchaseOrders.Queries.GetAll;

public class GetPurchaseOrdersQueryValidator
    : AbstractValidator<GetPurchaseOrdersQuery>
{
    public GetPurchaseOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(
                1,
                PaginationParameters.MaxPageSize);
    }
}