using MediatR;

namespace PPEInventory.Application.Features.Employees.Queries.GetAll;

public record GetEmployeesQuery
    : IRequest<IReadOnlyList<EmployeeDto>>;