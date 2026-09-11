using MediatR;

namespace PPEInventory.Application.Features.Users.Queries.GetById;

public record GetUserByIdQuery(
    int Id)
    : IRequest<UserDto>;