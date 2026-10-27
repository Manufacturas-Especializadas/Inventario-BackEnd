using MediatR;

namespace PPEInventory.Application.Features.Users.Commands.SetManagedOrganizationalUnits;

public record SetUserManagedOrganizationalUnitsCommand(
    int Id, IReadOnlyCollection<int> OrganizationalUnitIds) : IRequest;
