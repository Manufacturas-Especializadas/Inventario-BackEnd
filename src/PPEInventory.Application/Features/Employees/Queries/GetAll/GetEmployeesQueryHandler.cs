using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Employees.Queries.GetAll;

public class GetEmployeesQueryHandler
    : IRequestHandler<
        GetEmployeesQuery,
        IReadOnlyList<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IReadOnlyList<EmployeeDto>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees =
            await _employeeRepository.GetAllAsync(
                cancellationToken);

        return employees
            .Select(x => new EmployeeDto
            {
                Id = x.Id,
                EmployeeNumber = x.EmployeeNumber,
                Name = x.Name,

                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department.Name,

                LineId = x.LineId,
                LineName = x.Line?.Name,

                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}