using MediatR;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Users.Queries.GetManagedOrganizationalUnits;

public class GetUserManagedOrganizationalUnitsQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetUserManagedOrganizationalUnitsQuery, IReadOnlyList<ManagedOrganizationalUnitDto>>
{
    public async Task<IReadOnlyList<ManagedOrganizationalUnitDto>> Handle(
        GetUserManagedOrganizationalUnitsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue || !currentUser.IsInRole(AppRoles.Administrator))
            throw new ForbiddenException("Only Administrator can view users' managed units.");

        var user = await userRepository.GetByIdWithManagedUnitsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"User with id '{request.Id}' was not found.");

        return user.ManagedOrganizationalUnits.OrderBy(x => x.OrganizationalUnit.Name)
            .Select(x => new ManagedOrganizationalUnitDto
            {
                Id = x.OrganizationalUnitId,
                Name = x.OrganizationalUnit.Name,
                IsActive = x.OrganizationalUnit.IsActive
            }).ToArray();
    }
}
