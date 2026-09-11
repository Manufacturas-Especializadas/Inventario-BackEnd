using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Users.Commands.SetStatus;

public class SetUserStatusCommandHandler
    : IRequestHandler<
        SetUserStatusCommand,
        UserDto>
{
    private readonly IUserRepository
        _userRepository;

    private readonly ICurrentUserService
        _currentUserService;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public SetUserStatusCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository =
            userRepository;

        _currentUserService =
            currentUserService;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<UserDto> Handle(
        SetUserStatusCommand request,
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


        /*
         * Evita que el administrador
         * se bloquee a sí mismo.
         */
        if (
            !request.IsActive &&
            _currentUserService.UserId ==
                user.Id
        )
        {
            throw new ConflictException(
                "You cannot deactivate your own user account.");
        }


        /*
         * No tiene sentido activar una
         * cuenta cuyo empleado está inactivo.
         */
        if (
            request.IsActive &&
            !user.Employee.IsActive
        )
        {
            throw new ConflictException(
                $"User cannot be activated because employee '{user.Employee.EmployeeNumber}' is inactive.");
        }


        if (
            user.IsActive !=
            request.IsActive)
        {
            user.IsActive =
                request.IsActive;

            user.UpdatedAt =
                _dateTimeProvider.UtcNow;


            await _userRepository
                .SaveChangesAsync(
                    cancellationToken);
        }


        return user.ToDto();
    }
}