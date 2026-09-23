using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Users.Commands.ResetPassword;

public class ResetUserPasswordCommandHandler
    : IRequestHandler<
        ResetUserPasswordCommand,
        UserDto>
{
    private readonly IUserRepository
        _userRepository;

    private readonly IPasswordHasher
        _passwordHasher;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public ResetUserPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository =
            userRepository;

        _passwordHasher =
            passwordHasher;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<UserDto> Handle(
        ResetUserPasswordCommand request,
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


        user.PasswordHash =
            _passwordHasher.Hash(
                request.NewPassword);


        user.UpdatedAt =
            _dateTimeProvider.UtcNow;


        await _userRepository
            .SaveChangesAsync(
                cancellationToken);


        return user.ToDto();
    }
}