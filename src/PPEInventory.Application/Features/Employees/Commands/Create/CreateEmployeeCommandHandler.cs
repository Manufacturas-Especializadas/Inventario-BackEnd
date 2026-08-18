using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IProductionLineRepository productionLineRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _productionLineRepository = productionLineRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<EmployeeDto> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employeeNumber = request.EmployeeNumber.Trim();
        var employeeName = request.Name.Trim();

        var employeeAlreadyExists =
            await _employeeRepository.ExistsByEmployeeNumberAsync(
                employeeNumber,
                cancellationToken);

        if (employeeAlreadyExists)
        {
            throw new ConflictException(
                $"Employee number '{employeeNumber}' already exists.");
        }

        var department =
            await _departmentRepository.GetByIdAsync(
                request.DepartmentId,
                cancellationToken);

        if (department is null)
        {
            throw new NotFoundException(
                $"Department with id '{request.DepartmentId}' was not found.");
        }

        if (!department.IsActive)
        {
            throw new ConflictException(
                $"Department '{department.Name}' is inactive.");
        }

        ProductionLine? productionLine = null;

        if (request.LineId.HasValue)
        {
            productionLine =
                await _productionLineRepository.GetByIdAsync(
                    request.LineId.Value,
                    cancellationToken);

            if (productionLine is null)
            {
                throw new NotFoundException(
                    $"Production line with id '{request.LineId.Value}' was not found.");
            }

            if (!productionLine.IsActive)
            {
                throw new ConflictException(
                    $"Production line '{productionLine.Name}' is inactive.");
            }

            if (productionLine.DepartmentId != department.Id)
            {
                throw new ConflictException(
                    $"Production line '{productionLine.Name}' does not belong to department '{department.Name}'.");
            }
        }

        var employee = new Employee
        {
            EmployeeNumber = employeeNumber,
            Name = employeeName,
            DepartmentId = department.Id,
            LineId = productionLine?.Id,
            IsActive = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _employeeRepository.AddAsync(
            employee,
            cancellationToken);

        await _employeeRepository.SaveChangesAsync(
            cancellationToken);

        return new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            Name = employee.Name,

            DepartmentId = department.Id,
            DepartmentName = department.Name,

            LineId = productionLine?.Id,
            LineName = productionLine?.Name,

            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };
    }
}