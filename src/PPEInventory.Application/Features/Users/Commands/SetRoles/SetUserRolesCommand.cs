using MediatR;

namespace PPEInventory.Application.Features.Users.Commands.SetRoles;

public record SetUserRolesCommand(
    int Id,
    IReadOnlyCollection<string> Roles)
    : IRequest<UserDto>;