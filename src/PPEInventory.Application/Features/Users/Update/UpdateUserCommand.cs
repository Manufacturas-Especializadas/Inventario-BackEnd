using MediatR;

namespace PPEInventory.Application.Features.Users.Commands.Update;

public record UpdateUserCommand(
    int Id,
    string Username)
    : IRequest<UserDto>;