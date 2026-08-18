using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Users.Commands.Create;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var employee =
            await _employeeRepository
                .GetByEmployeeNumberAsync(
                    employeeNumber,
                    cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                $"Employee '{employeeNumber}' was not found.");
        }

        if (!employee.IsActive)
        {
            throw new ConflictException(
                $"Employee '{employeeNumber}' is inactive.");
        }

        if (await _userRepository.ExistsByUsernameAsync(
            username,
            cancellationToken))
        {
            throw new ConflictException(
                $"Username '{username}' already exists.");
        }

        if (await _userRepository.ExistsByEmployeeIdAsync(
            employee.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"Employee '{employeeNumber}' already has a user.");
        }

        var requestedRoles = request.Roles
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var roles =
            await _roleRepository.GetByNamesAsync(
                requestedRoles,
                cancellationToken);

        var existingRoleNames = roles
            .Select(x => x.Name)
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var missingRoles = requestedRoles
            .Where(x => !existingRoleNames.Contains(x))
            .ToArray();

        if (missingRoles.Length > 0)
        {
            throw new NotFoundException(
                $"Role(s) not found: {string.Join(", ", missingRoles)}.");
        }

        var user = new User
        {
            EmployeeId = employee.Id,

            Username = username,

            PasswordHash =
                _passwordHasher.Hash(
                    request.Password),

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            UserRoles = roles
                .Select(role => new UserRole
                {
                    RoleId = role.Id
                })
                .ToList()
        };

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _userRepository.SaveChangesAsync(
            cancellationToken);

        return user.Id;
    }
}