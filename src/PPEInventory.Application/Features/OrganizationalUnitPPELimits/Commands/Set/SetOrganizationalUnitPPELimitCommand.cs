using MediatR;

namespace PPEInventory.Application.Features.OrganizationalUnitPPELimits.Commands.Set;

public record SetOrganizationalUnitPPELimitCommand(
    int OrganizationalUnitId,
    int PPEProductId,
    int MaxQuantityPerCycle,
    bool IsActive = true)
    : IRequest<
        OrganizationalUnitPPELimitDto>;