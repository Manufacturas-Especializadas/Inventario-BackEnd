using MediatR;

namespace PPEInventory.Application.Features.OrganizationalUnits.Queries.GetAll;

public record GetOrganizationalUnitsQuery()
    : IRequest<
        IReadOnlyList<OrganizationalUnitDto>>;