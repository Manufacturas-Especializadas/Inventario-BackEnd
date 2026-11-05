using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Notifications;

internal sealed class RoleNotificationRecipientResolver(ApplicationDbContext context)
    : INotificationRecipientResolver
{
    public async Task<IReadOnlyCollection<string>> GetActiveEmailsByRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var emails = await context.Users
            .AsNoTracking()
            .Where(user => user.IsActive && user.Employee.IsActive
                && user.Employee.Email != null && user.Employee.Email != ""
                && user.UserRoles.Any(userRole =>
                    userRole.Role.IsActive && userRole.Role.Name == roleName))
            .Select(user => user.Employee.Email!)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Normalize legacy values and deduplicate independently of SQL collation.
        return emails
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
