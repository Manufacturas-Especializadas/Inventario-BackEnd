using MediatR;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Users.Commands.SetRoles;

public class SetUserRolesCommandHandler
    : IRequestHandler<
        SetUserRolesCommand,
        UserDto>
{
    private readonly IUserRepository
        _userRepository;

    private readonly IRoleRepository
        _roleRepository;

    private readonly ICurrentUserService
        _currentUserService;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public SetUserRolesCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository =
            userRepository;

        _roleRepository =
            roleRepository;

        _currentUserService =
            currentUserService;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<UserDto> Handle(
        SetUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository
                .GetByIdForUpdateWithDetailsAsync(
                    request.Id,
                    cancellationToken);


        if (user is null)
        {
            throw new NotFoundException(
                $"User with id '{request.Id}' was not found.");
        }


        var requestedRoleNames =
            request.Roles
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                    x.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();


        if (requestedRoleNames.Length == 0)
        {
            throw new ConflictException(
                "At least one role must be assigned.");
        }


        /*
         * Un administrador no puede quitarse
         * a sí mismo el rol Administrator.
         */
        if (
            _currentUserService.UserId ==
                user.Id &&
            !requestedRoleNames.Contains(
                AppRoles.Administrator,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ConflictException(
                "You cannot remove the Administrator role from your own account.");
        }


        var roles =
            await _roleRepository
                .GetByNamesAsync(
                    requestedRoleNames,
                    cancellationToken);


        var existingRoleNames =
            roles
                .Select(x => x.Name)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);


        var missingRoles =
            requestedRoleNames
                .Where(x =>
                    !existingRoleNames
                        .Contains(x))
                .ToArray();


        if (missingRoles.Length > 0)
        {
            throw new NotFoundException(
                $"Role(s) not found or inactive: {string.Join(", ", missingRoles)}.");
        }


        var requestedRoleIds =
            roles
                .Select(x => x.Id)
                .ToHashSet();


        var currentRoleIds =
            user.UserRoles
                .Select(x => x.RoleId)
                .ToHashSet();


        /*
         * Si exactamente los mismos roles
         * ya están asignados, no hacemos nada.
         */
        if (
            currentRoleIds.SetEquals(
                requestedRoleIds))
        {
            return user.ToDto();
        }


        /*
         * Quitar solamente relaciones
         * que ya no son necesarias.
         */
        var rolesToRemove =
            user.UserRoles
                .Where(x =>
                    !requestedRoleIds
                        .Contains(
                            x.RoleId))
                .ToArray();


        foreach (
            var userRole
            in rolesToRemove)
        {
            user.UserRoles.Remove(
                userRole);
        }


        /*
         * IDs que permanecen después
         * de las eliminaciones.
         */
        var remainingRoleIds =
            user.UserRoles
                .Select(x => x.RoleId)
                .ToHashSet();


        /*
         * Agregar únicamente roles nuevos.
         */
        foreach (
            var role
            in roles.Where(x =>
                !remainingRoleIds
                    .Contains(x.Id)))
        {
            user.UserRoles.Add(
                new UserRole
                {
                    UserId =
                        user.Id,

                    RoleId =
                        role.Id,

                    Role =
                        role
                });
        }


        user.UpdatedAt =
            _dateTimeProvider.UtcNow;


        await _userRepository
            .SaveChangesAsync(
                cancellationToken);


        return user.ToDto();
    }
}