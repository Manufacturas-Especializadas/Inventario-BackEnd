using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Departments.Commands.Create;

public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _departmentRepository = departmentRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<DepartmentDto> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim();

        var alreadyExists =
            await _departmentRepository.ExistsByNameAsync(
                normalizedName,
                cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Department '{normalizedName}' already exists.");
        }

        var department = new Department
        {
            Name = normalizedName,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),

            IsActive = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _departmentRepository.AddAsync(
            department,
            cancellationToken);

        await _departmentRepository.SaveChangesAsync(
            cancellationToken);

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt
        };
    }
}