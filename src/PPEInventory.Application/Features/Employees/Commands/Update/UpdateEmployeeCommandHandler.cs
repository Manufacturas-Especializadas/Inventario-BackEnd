using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommandHandler
    : IRequestHandler<
        UpdateEmployeeCommand,
        EmployeeDto>
{
    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly IOrganizationalUnitRepository
        _organizationalUnitRepository;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IOrganizationalUnitRepository
            organizationalUnitRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _employeeRepository =
            employeeRepository;

        _organizationalUnitRepository =
            organizationalUnitRepository;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<EmployeeDto> Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var employeeName =
            request.Name.Trim();


        var employee =
            await _employeeRepository
                .GetByIdForUpdateAsync(
                    request.Id,
                    cancellationToken);


        if (employee is null)
        {
            throw new NotFoundException(
                $"Employee with id '{request.Id}' was not found.");
        }


        /*
         * Solo consultamos duplicado si realmente
         * se está cambiando el número.
         */
        if (!string.Equals(
                employee.EmployeeNumber,
                employeeNumber,
                StringComparison.OrdinalIgnoreCase))
        {
            var employeeNumberExists =
                await _employeeRepository
                    .ExistsByEmployeeNumberAsync(
                        employeeNumber,
                        cancellationToken);

            if (employeeNumberExists)
            {
                throw new ConflictException(
                    $"Employee number '{employeeNumber}' already exists.");
            }
        }


        var organizationalUnit =
            await _organizationalUnitRepository
                .GetByIdAsync(
                    request.OrganizationalUnitId,
                    cancellationToken);


        if (organizationalUnit is null)
        {
            throw new NotFoundException(
                $"Organizational unit with id '{request.OrganizationalUnitId}' was not found.");
        }


        if (!organizationalUnit.IsActive)
        {
            throw new ConflictException(
                $"Organizational unit '{organizationalUnit.Name}' is inactive.");
        }


        employee.EmployeeNumber =
            employeeNumber;

        employee.Name =
            employeeName;

        employee.OrganizationalUnitId =
            organizationalUnit.Id;


        /*
         * Legacy:
         * OrganizationalUnit sigue siendo
         * la fuente de verdad.
         */
        employee.DepartmentId =
            null;

        employee.LineId =
            null;


        employee.UpdatedAt =
            _dateTimeProvider.UtcNow;


        await _employeeRepository
            .SaveChangesAsync(
                cancellationToken);


        return new EmployeeDto
        {
            Id =
                employee.Id,

            EmployeeNumber =
                employee.EmployeeNumber,

            Name =
                employee.Name,

            DepartmentId =
                null,

            DepartmentName =
                null,

            LineId =
                null,

            LineName =
                null,

            OrganizationalUnitId =
                organizationalUnit.Id,

            OrganizationalUnitName =
                organizationalUnit.Name,

            OrganizationalUnitType =
                organizationalUnit.Type,

            IsActive =
                employee.IsActive,

            CreatedAt =
                employee.CreatedAt
        };
    }
}