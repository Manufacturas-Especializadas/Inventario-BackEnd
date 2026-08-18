using MediatR;

namespace PPEInventory.Application.Features.Employees.Queries.GetByEmployeeNumber;

public record GetEmployeeByNumberQuery(
    string EmployeeNumber)
    : IRequest<EmployeeDto>;