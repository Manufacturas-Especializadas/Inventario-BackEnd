using MediatR;

namespace PPEInventory.Application.Features.Employees.Commands.SetStatus;

public record SetEmployeeStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<EmployeeDto>;