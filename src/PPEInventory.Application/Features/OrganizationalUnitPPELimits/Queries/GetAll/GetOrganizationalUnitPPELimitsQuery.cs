using MediatR;

namespace PPEInventory.Application.Features.OrganizationalUnitPPELimits.Queries.GetAll;

public record GetOrganizationalUnitPPELimitsQuery(
    int? OrganizationalUnitId,
    int? PPEProductId)
    : IRequest<
        IReadOnlyList<
            OrganizationalUnitPPELimitDto>>;