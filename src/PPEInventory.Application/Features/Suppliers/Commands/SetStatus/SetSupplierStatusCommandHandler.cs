using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Suppliers.Commands.SetStatus;

public class SetSupplierStatusCommandHandler
    : IRequestHandler<
        SetSupplierStatusCommand,
        SupplierDto>
{
    private readonly ISupplierRepository
        _repository;

    private readonly ICurrentUserService
        _currentUser;

    private readonly IDateTimeProvider
        _dateTimeProvider;


    public SetSupplierStatusCommandHandler(
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
        SetSupplierStatusCommand request,
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


        if (
            supplier.IsActive !=
            request.IsActive)
        {
            var userId =
                _currentUser.UserId
                ?? throw new UnauthorizedException(
                    "Authenticated user was not found.");

            supplier.IsActive =
                request.IsActive;

            supplier.UpdatedAt =
                _dateTimeProvider.UtcNow;

            supplier.UpdatedByUserId =
                userId;

            await _repository
                .SaveChangesAsync(
                    cancellationToken);
        }


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
}