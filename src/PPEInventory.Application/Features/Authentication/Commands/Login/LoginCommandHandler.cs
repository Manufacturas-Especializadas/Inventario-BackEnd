using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();

        var user =
            await _userRepository.GetByUsernameWithRolesAsync(
                username,
                cancellationToken);

        if (user is null ||
            !user.IsActive ||
            !user.Employee.IsActive)
        {
            throw new UnauthorizedException(
                "Invalid username or password.");
        }

        var passwordIsValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new UnauthorizedException(
                "Invalid username or password.");
        }

        var roles = user.UserRoles
            .Where(x => x.Role.IsActive)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToArray();

        user.LastLoginAt =
            _dateTimeProvider.UtcNow;

        await _userRepository.SaveChangesAsync(
            cancellationToken);

        var token =
            _jwtTokenGenerator.GenerateToken(
                user,
                roles);

        return new LoginResponseDto
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,

            UserId = user.Id,

            EmployeeId = user.EmployeeId,

            EmployeeNumber =
                user.Employee.EmployeeNumber,

            Name = user.Employee.Name,

            Username = user.Username,

            Roles = roles
        };
    }
}