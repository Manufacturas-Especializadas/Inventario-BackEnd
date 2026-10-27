using MediatR;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.Users.Commands.SetManagedOrganizationalUnits;

public class SetUserManagedOrganizationalUnitsCommandHandler(
    IUserRepository userRepository,
    IOrganizationalUnitRepository organizationalUnitRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<SetUserManagedOrganizationalUnitsCommand>
{
    public async Task Handle(
        SetUserManagedOrganizationalUnitsCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue || !currentUser.IsInRole(AppRoles.Administrator))
            throw new ForbiddenException("Only Administrator can modify managed organizational units.");

        await using var transaction = await unitOfWork.BeginSerializableTransactionAsync(cancellationToken);
        try
        {
            var user = await userRepository.GetByIdWithManagedUnitsAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"User with id '{request.Id}' was not found.");
            var ids = request.OrganizationalUnitIds.ToHashSet();
            foreach (var id in ids.Order())
            {
                var unit = await organizationalUnitRepository.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException($"Organizational unit with id '{id}' was not found.");
                if (!unit.IsActive)
                    throw new ConflictException($"Organizational unit '{unit.Name}' is inactive.");
            }

            foreach (var assignment in user.ManagedOrganizationalUnits.Where(x => !ids.Contains(x.OrganizationalUnitId)).ToArray())
                user.ManagedOrganizationalUnits.Remove(assignment);

            var existingIds = user.ManagedOrganizationalUnits.Select(x => x.OrganizationalUnitId).ToHashSet();
            foreach (var id in ids.Except(existingIds))
                user.ManagedOrganizationalUnits.Add(new UserManagedOrganizationalUnit
                {
                    UserId = user.Id,
                    OrganizationalUnitId = id
                });

            user.UpdatedAt = dateTimeProvider.UtcNow;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
