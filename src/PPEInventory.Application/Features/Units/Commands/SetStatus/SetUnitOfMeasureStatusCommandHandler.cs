using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Units.Commands.SetStatus;

public class SetUnitOfMeasureStatusCommandHandler
    : IRequestHandler<
        SetUnitOfMeasureStatusCommand,
        UnitOfMeasureDto>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SetUnitOfMeasureStatusCommandHandler(
        IUnitOfMeasureRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<UnitOfMeasureDto> Handle(
        SetUnitOfMeasureStatusCommand request,
        CancellationToken cancellationToken)
    {
        var unit =
            await _repository.GetByIdForUpdateAsync(
                request.Id,
                cancellationToken);

        if (unit is null)
        {
            throw new NotFoundException(
                $"Unit with id '{request.Id}' was not found.");
        }

        if (unit.IsActive == request.IsActive)
        {
            return Map(unit);
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        unit.IsActive =
            request.IsActive;

        unit.UpdatedAt =
            _dateTimeProvider.UtcNow;

        unit.UpdatedByUserId =
            userId;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(unit);
    }

    private static UnitOfMeasureDto Map(
        PPEInventory.Domain.Entities.UnitOfMeasure unit)
    {
        return new UnitOfMeasureDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Symbol = unit.Symbol,
            IsActive = unit.IsActive,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }
}