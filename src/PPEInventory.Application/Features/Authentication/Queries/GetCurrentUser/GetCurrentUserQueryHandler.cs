using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Authentication.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IEmployeeRepository _employeeRepository;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUser,
        IEmployeeRepository employeeRepository)
    {
        _currentUser = currentUser;
        _employeeRepository = employeeRepository;
    }

    public async Task<CurrentUserDto> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            !_currentUser.UserId.HasValue ||
            !_currentUser.EmployeeId.HasValue)
        {
            throw new UnauthorizedException("Authenticated user was not found.");
        }

        var employee = await _employeeRepository.GetIdentityByIdAsync(
            _currentUser.EmployeeId.Value,
            cancellationToken)
            ?? throw new UnauthorizedException("Authenticated employee was not found.");

        return new CurrentUserDto
        {
            IsAuthenticated = true,
            UserId = _currentUser.UserId.Value,
            EmployeeId = employee.EmployeeId,
            EmployeeNumber = employee.EmployeeNumber,
            Name = employee.Name,
            Username = _currentUser.Username,
            Roles = _currentUser.Roles,
            OrganizationalUnitId = employee.OrganizationalUnitId,
            OrganizationalUnitName = employee.OrganizationalUnitName,
            OrganizationalUnitType = employee.OrganizationalUnitType
        };
    }
}
