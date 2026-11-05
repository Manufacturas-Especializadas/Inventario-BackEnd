using MediatR;

namespace PPEInventory.Application.Features.Employees.Commands.Create;

public record CreateEmployeeCommand(
    string EmployeeNumber,
    string Name,
    string? Email,
    int? OrganizationalUnitId)
    : IRequest<EmployeeDto>;