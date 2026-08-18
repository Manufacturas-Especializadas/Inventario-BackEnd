using MediatR;

namespace PPEInventory.Application.Features.Authentication.Commands.Login;

public record LoginCommand(
    string Username,
    string Password)
    : IRequest<LoginResponseDto>;