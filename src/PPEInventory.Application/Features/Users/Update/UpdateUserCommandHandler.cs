using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Users.Commands.Update;

public class UpdateUserCommandHandler
    : IRequestHandler<
        UpdateUserCommand,
        UserDto>
{
    private readonly IUserRepository
        _userRepository;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository =
            userRepository;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<UserDto> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var username =
            request.Username.Trim();

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


        if (!string.Equals(
                user.Username,
                username,
                StringComparison.OrdinalIgnoreCase))
        {
            var usernameExists =
                await _userRepository
                    .ExistsByUsernameAsync(
                        username,
                        cancellationToken);

            if (usernameExists)
            {
                throw new ConflictException(
                    $"Username '{username}' already exists.");
            }
        }


        user.Username =
            username;

        user.UpdatedAt =
            _dateTimeProvider.UtcNow;


        await _userRepository
            .SaveChangesAsync(
                cancellationToken);


        return user.ToDto();
    }
}