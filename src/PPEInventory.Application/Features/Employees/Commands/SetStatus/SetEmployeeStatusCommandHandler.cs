using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Employees.Commands.SetStatus;

public class SetEmployeeStatusCommandHandler
    : IRequestHandler<
        SetEmployeeStatusCommand,
        EmployeeDto>
{
    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public SetEmployeeStatusCommandHandler(
        IEmployeeRepository employeeRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _employeeRepository =
            employeeRepository;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<EmployeeDto> Handle(
        SetEmployeeStatusCommand request,
        CancellationToken cancellationToken)
    {
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
         * Para reactivar un empleado,
         * debe seguir perteneciendo a una
         * unidad organizacional válida y activa.
         */
        if (request.IsActive)
        {
            if (
                employee.OrganizationalUnitId
                    is null ||
                employee.OrganizationalUnit
                    is null)
            {
                throw new ConflictException(
                    "The employee cannot be activated because no organizational unit is assigned.");
            }


            if (
                !employee
                    .OrganizationalUnit
                    .IsActive)
            {
                throw new ConflictException(
                    $"The employee cannot be activated because organizational unit '{employee.OrganizationalUnit.Name}' is inactive.");
            }
        }


        /*
         * El endpoint es idempotente:
         * pedir Activo cuando ya está activo
         * simplemente devuelve el estado actual.
         */
        if (
            employee.IsActive !=
            request.IsActive)
        {
            employee.IsActive =
                request.IsActive;

            employee.UpdatedAt =
                _dateTimeProvider.UtcNow;


            await _employeeRepository
                .SaveChangesAsync(
                    cancellationToken);
        }


        return new EmployeeDto
        {
            Id =
                employee.Id,

            EmployeeNumber =
                employee.EmployeeNumber,

            Name =
                employee.Name,

            DepartmentId =
                employee.DepartmentId,

            DepartmentName =
                employee.Department?.Name,

            LineId =
                employee.LineId,

            LineName =
                employee.Line?.Name,

            OrganizationalUnitId =
                employee.OrganizationalUnitId,

            OrganizationalUnitName =
                employee
                    .OrganizationalUnit
                    ?.Name,

            OrganizationalUnitType =
                employee
                    .OrganizationalUnit
                    ?.Type,

            IsActive =
                employee.IsActive,

            CreatedAt =
                employee.CreatedAt
        };
    }
}