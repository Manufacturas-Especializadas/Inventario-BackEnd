using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Employees.Queries.GetByEmployeeNumber;

public class GetEmployeeByNumberQueryHandler
    : IRequestHandler<GetEmployeeByNumberQuery, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByNumberQueryHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto> Handle(
        GetEmployeeByNumberQuery request,
        CancellationToken cancellationToken)
    {
        var employeeNumber = request.EmployeeNumber.Trim();

        var employee =
            await _employeeRepository.GetByEmployeeNumberAsync(
                employeeNumber,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                $"Employee '{employeeNumber}' was not found.");
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            Name = employee.Name,

            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,

            LineId = employee.LineId,
            LineName = employee.Line?.Name,

            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };
    }
}