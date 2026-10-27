using MediatR;

namespace PPEInventory.Application.Features.Users.Queries.GetManagedOrganizationalUnits;

public record GetUserManagedOrganizationalUnitsQuery(int Id)
    : IRequest<IReadOnlyList<ManagedOrganizationalUnitDto>>;
