using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Warehouses.Commands.Create;

public class CreateWarehouseCommandHandler
    : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
{
    private readonly IWarehouseRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateWarehouseCommandHandler(
        IWarehouseRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WarehouseDto> Handle(
        CreateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        var code =
            request.Code.Trim().ToUpperInvariant();

        if (await _repository.ExistsByCodeAsync(
            code,
            cancellationToken))
        {
            throw new ConflictException(
                $"Warehouse code '{code}' already exists.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var warehouse = new Warehouse
        {
            Code = code,
            Name = request.Name.Trim(),

            Description =
                string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            CreatedByUserId = userId
        };

        await _repository.AddAsync(
            warehouse,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Description = warehouse.Description,
            IsActive = warehouse.IsActive,
            CreatedAt = warehouse.CreatedAt
        };
    }
}