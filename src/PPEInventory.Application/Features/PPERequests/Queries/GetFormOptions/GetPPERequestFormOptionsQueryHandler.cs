using MediatR;
using PPEInventory.Application.Features.RequestReasons;
using PPEInventory.Application.Features.Users;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetFormOptions;

public class GetPPERequestFormOptionsQueryHandler
    : IRequestHandler<GetPPERequestFormOptionsQuery, PPERequestFormOptionsDto>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IRequestReasonRepository _reasonRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public GetPPERequestFormOptionsQueryHandler(
        IWarehouseRepository warehouseRepository,
        IRequestReasonRepository reasonRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _warehouseRepository = warehouseRepository;
        _reasonRepository = reasonRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<PPERequestFormOptionsDto> Handle(
        GetPPERequestFormOptionsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ManagedOrganizationalUnitDto> managedUnits = Array.Empty<ManagedOrganizationalUnitDto>();
        if (!_currentUser.IsInRole(AppRoles.Administrator))
        {
            if (!_currentUser.IsInRole(AppRoles.Production) || !_currentUser.UserId.HasValue)
                throw new ForbiddenException("Only Administrator or Production can request PPE form options.");
            var user = await _userRepository.GetByIdWithManagedUnitsAsync(_currentUser.UserId.Value, cancellationToken)
                ?? throw new UnauthorizedException("Authenticated user was not found.");
            if (!user.IsActive)
                throw new ForbiddenException("The authenticated user is inactive.");
            managedUnits = user.ManagedOrganizationalUnits
                .Where(x => x.OrganizationalUnit.IsActive)
                .OrderBy(x => x.OrganizationalUnit.Name)
                .Select(x => new ManagedOrganizationalUnitDto
                {
                    Id = x.OrganizationalUnitId,
                    Name = x.OrganizationalUnit.Name,
                    IsActive = true
                }).ToArray();
        }

        var warehouses = await _warehouseRepository.GetPPERequestOptionsAsync(
            cancellationToken);
        var reasons = await _reasonRepository.GetAllAsync(cancellationToken);

        return new PPERequestFormOptionsDto
        {
            ManagedOrganizationalUnits = managedUnits,
            Warehouses = warehouses,
            RequestReasons = reasons.Select(x => new RequestReasonDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            }).ToArray()
        };
    }
}
