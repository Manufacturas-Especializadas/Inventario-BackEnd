using MediatR;

namespace PPEInventory.Application.Features.Users.Commands.ResetPassword;

public record ResetUserPasswordCommand(
    int Id,
    string NewPassword)
    : IRequest<UserDto>;