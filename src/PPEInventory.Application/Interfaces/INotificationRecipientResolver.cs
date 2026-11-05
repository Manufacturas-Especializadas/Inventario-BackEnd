namespace PPEInventory.Application.Interfaces;

public interface INotificationRecipientResolver
{
    Task<IReadOnlyCollection<string>> GetActiveEmailsByRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default);
}
