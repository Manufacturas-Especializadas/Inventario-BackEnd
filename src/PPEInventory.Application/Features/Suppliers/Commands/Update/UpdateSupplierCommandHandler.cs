using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommandHandler
    : IRequestHandler<
        UpdateSupplierCommand,
        SupplierDto>
{
    private readonly ISupplierRepository
        _repository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public UpdateSupplierCommandHandler(
        ISupplierRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository =
            repository;

        _currentUser =
            currentUser;

        _dateTimeProvider =
            dateTimeProvider;
    }


    public async Task<SupplierDto> Handle(
        UpdateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var supplier =
            await _repository
                .GetByIdForUpdateAsync(
                    request.Id,
                    cancellationToken);

        if (supplier is null)
        {
            throw new NotFoundException(
                $"Supplier with id '{request.Id}' was not found.");
        }


        var name =
            request.Name.Trim();

        var contactName =
            Normalize(
                request.ContactName);

        var email =
            Normalize(
                request.Email);

        var phone =
            Normalize(
                request.Phone);


        var duplicateExists =
            await _repository
                .ExistsByNameAsync(
                    name,
                    supplier.Id,
                    cancellationToken);

        if (duplicateExists)
        {
            throw new ConflictException(
                $"Supplier '{name}' already exists.");
        }


        var nothingChanged =
            string.Equals(
                supplier.Name,
                name,
                StringComparison.Ordinal) &&

            string.Equals(
                supplier.ContactName,
                contactName,
                StringComparison.Ordinal) &&

            string.Equals(
                supplier.Email,
                email,
                StringComparison.Ordinal) &&

            string.Equals(
                supplier.Phone,
                phone,
                StringComparison.Ordinal);


        if (nothingChanged)
        {
            return Map(supplier);
        }


        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");


        supplier.Name =
            name;

        supplier.ContactName =
            contactName;

        supplier.Email =
            email;

        supplier.Phone =
            phone;

        supplier.UpdatedAt =
            _dateTimeProvider.UtcNow;

        supplier.UpdatedByUserId =
            userId;


        await _repository
            .SaveChangesAsync(
                cancellationToken);


        return Map(supplier);
    }


    private static SupplierDto Map(
        Domain.Entities.Supplier supplier)
    {
        return new SupplierDto
        {
            Id =
                supplier.Id,

            Name =
                supplier.Name,

            ContactName =
                supplier.ContactName,

            Email =
                supplier.Email,

            Phone =
                supplier.Phone,

            IsActive =
                supplier.IsActive,

            CreatedAt =
                supplier.CreatedAt
        };
    }


    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }
}