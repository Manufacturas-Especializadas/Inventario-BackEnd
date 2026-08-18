using MediatR;

namespace PPEInventory.Application.Features.Users.Commands.Create;

public record CreateUserCommand(
    string EmployeeNumber,
    string Username,
    string Password,
    IReadOnlyCollection<string> Roles)
    : IRequest<int>;