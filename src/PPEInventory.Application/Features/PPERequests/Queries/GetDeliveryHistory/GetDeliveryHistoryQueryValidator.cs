using FluentValidation;
using PPEInventory.Application.Common.Models;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetDeliveryHistory;

public class GetDeliveryHistoryQueryValidator
    : AbstractValidator<GetDeliveryHistoryQuery>
{
    public GetDeliveryHistoryQueryValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .MaximumLength(50);

        RuleFor(x => x.Search)
            .MaximumLength(150);

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .When(x =>
                x.WarehouseId.HasValue);

        RuleFor(
                x =>
                    x.RequestedForOrganizationalUnitId)
            .GreaterThan(0)
            .When(
                x =>
                    x.RequestedForOrganizationalUnitId
                        .HasValue);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(
                1,
                PaginationParameters.MaxPageSize);

        RuleFor(x => x)
            .Must(
                x =>
                    x.DeliveredFrom <=
                    x.DeliveredTo)
            .WithMessage(
                "DeliveredFrom cannot be greater than DeliveredTo.");

        RuleFor(x => x)
            .Must(
                x =>
                    ((long)x.PageNumber - 1) *
                    x.PageSize <=
                    int.MaxValue)
            .When(
                x =>
                    x.PageNumber > 0 &&
                    x.PageSize > 0)
            .WithMessage(
                "The requested pagination offset is too large.");
    }
}