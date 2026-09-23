using MediatR;

namespace PPEInventory.Application.Features.Users.Commands.SetStatus;

public record SetUserStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<UserDto>;