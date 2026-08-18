using MediatR;

namespace PPEInventory.Application.Features.Employees.Commands.Create;

public record CreateEmployeeCommand(
    string EmployeeNumber,
    string Name,
    int DepartmentId,
    int? LineId)
    : IRequest<EmployeeDto>;