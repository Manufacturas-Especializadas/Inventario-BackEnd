using MediatR;

namespace PPEInventory.Application.Features.Employees.Commands.Update;

public record UpdateEmployeeCommand(
    int Id,
    string EmployeeNumber,
    string Name,
    string? Email,
    int? OrganizationalUnitId)
    : IRequest<EmployeeDto>;