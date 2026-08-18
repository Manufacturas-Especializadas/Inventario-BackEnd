using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.ProductionLines.Commands.Create;

public class CreateProductionLineCommandHandler
    : IRequestHandler<
        CreateProductionLineCommand,
        ProductionLineDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateProductionLineCommandHandler(
        IDepartmentRepository departmentRepository,
        IProductionLineRepository productionLineRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _departmentRepository = departmentRepository;
        _productionLineRepository = productionLineRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductionLineDto> Handle(
        CreateProductionLineCommand request,
        CancellationToken cancellationToken)
    {
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

        var normalizedName = request.Name.Trim();

        var alreadyExists =
            await _productionLineRepository.ExistsAsync(
                request.DepartmentId,
                normalizedName,
                cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Production line '{normalizedName}' already exists in department '{department.Name}'.");
        }

        var productionLine = new ProductionLine
        {
            DepartmentId = department.Id,
            Name = normalizedName,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            IsActive = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _productionLineRepository.AddAsync(
            productionLine,
            cancellationToken);

        await _productionLineRepository.SaveChangesAsync(
            cancellationToken);

        return new ProductionLineDto
        {
            Id = productionLine.Id,
            DepartmentId = department.Id,
            DepartmentName = department.Name,
            Name = productionLine.Name,
            Description = productionLine.Description,
            IsActive = productionLine.IsActive,
            CreatedAt = productionLine.CreatedAt
        };
    }
}