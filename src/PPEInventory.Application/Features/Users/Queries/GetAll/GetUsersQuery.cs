using MediatR;

namespace PPEInventory.Application.Features.Users.Queries.GetAll;

public record GetUsersQuery()
    : IRequest<IReadOnlyList<UserDto>>;