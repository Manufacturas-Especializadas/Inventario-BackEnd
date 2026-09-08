using MediatR;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.OrganizationalUnits.Commands.Create;

public record CreateOrganizationalUnitCommand(
    string Name,
    string? Description,
    OrganizationalUnitType Type,
    int? ParentId)
    : IRequest<OrganizationalUnitDto>;