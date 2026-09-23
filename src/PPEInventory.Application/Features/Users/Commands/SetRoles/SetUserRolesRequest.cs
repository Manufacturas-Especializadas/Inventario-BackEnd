namespace PPEInventory.Application.Features.Users.Commands.SetRoles;

public record SetUserRolesRequest(
    IReadOnlyCollection<string> Roles);