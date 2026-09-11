using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Units.Commands.Create;

public class CreateUnitOfMeasureCommandHandler
    : IRequestHandler<
        CreateUnitOfMeasureCommand,
        UnitOfMeasureDto>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateUnitOfMeasureCommandHandler(
        IUnitOfMeasureRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<UnitOfMeasureDto> Handle(
        CreateUnitOfMeasureCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name,
            cancellationToken))
        {
            throw new ConflictException(
                $"A unit named '{name}' already exists.");
        }

        var userId =
            _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var unit = new UnitOfMeasure
        {
            Name = name,
            Symbol = Normalize(request.Symbol),

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            CreatedByUserId =
                userId
        };

        await _repository.AddAsync(
            unit,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(unit);
    }

    private static UnitOfMeasureDto Map(
        UnitOfMeasure unit)
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

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}