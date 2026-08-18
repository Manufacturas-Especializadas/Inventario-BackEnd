using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommandHandler
    : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateSupplierCommandHandler(
        ISupplierRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SupplierDto> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name,
            cancellationToken))
        {
            throw new ConflictException(
                $"Supplier '{name}' already exists.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var supplier = new Supplier
        {
            Name = name,

            ContactName =
                Normalize(request.ContactName),

            Email =
                Normalize(request.Email),

            Phone =
                Normalize(request.Phone),

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            CreatedByUserId = userId
        };

        await _repository.AddAsync(
            supplier,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone,
            IsActive = supplier.IsActive,
            CreatedAt = supplier.CreatedAt
        };
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}