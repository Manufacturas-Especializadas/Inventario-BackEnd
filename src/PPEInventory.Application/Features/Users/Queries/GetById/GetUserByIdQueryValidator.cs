using FluentValidation;

namespace PPEInventory.Application.Features.Users.Queries.GetById;

public class GetUserByIdQueryValidator
    : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}