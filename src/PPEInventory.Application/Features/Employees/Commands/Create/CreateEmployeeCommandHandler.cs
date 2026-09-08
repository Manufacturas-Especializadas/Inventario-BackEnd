using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommandHandler
    : IRequestHandler<
        CreateEmployeeCommand,
        EmployeeDto>
{
    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly IOrganizationalUnitRepository
        _organizationalUnitRepository;

    private readonly IDateTimeProvider
        _dateTimeProvider;

    public CreateEmployeeCommandHandler(
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
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employeeNumber =
            request.EmployeeNumber.Trim();

        var employeeName =
            request.Name.Trim();

        var employeeAlreadyExists =
            await _employeeRepository
                .ExistsByEmployeeNumberAsync(
                    employeeNumber,
                    cancellationToken);

        if (employeeAlreadyExists)
        {
            throw new ConflictException(
                $"Employee number '{employeeNumber}' already exists.");
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

        var employee =
            new Employee
            {
                EmployeeNumber =
                    employeeNumber,

                Name =
                    employeeName,

                OrganizationalUnitId =
                    organizationalUnit.Id,

                /*
                 * Legacy.
                 * Ya no son fuente de verdad.
                 */
                DepartmentId =
                    null,

                LineId =
                    null,

                IsActive =
                    true,

                CreatedAt =
                    _dateTimeProvider.UtcNow
            };

        await _employeeRepository.AddAsync(
            employee,
            cancellationToken);

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