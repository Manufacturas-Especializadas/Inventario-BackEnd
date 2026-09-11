using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Units.Commands.Update;

public class UpdateUnitOfMeasureCommandHandler
    : IRequestHandler<
        UpdateUnitOfMeasureCommand,
        UnitOfMeasureDto>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateUnitOfMeasureCommandHandler(
        IUnitOfMeasureRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<UnitOfMeasureDto> Handle(
        UpdateUnitOfMeasureCommand request,
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

        var name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name,
            unit.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"A unit named '{name}' already exists.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        unit.Name = name;
        unit.Symbol = Normalize(request.Symbol);

        unit.UpdatedAt =
            _dateTimeProvider.UtcNow;

        unit.UpdatedByUserId =
            userId;

        await _repository.SaveChangesAsync(
            cancellationToken);

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

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}