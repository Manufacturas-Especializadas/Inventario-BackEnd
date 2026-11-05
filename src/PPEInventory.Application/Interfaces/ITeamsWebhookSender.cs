using PPEInventory.Application.Common.Notifications;

namespace PPEInventory.Application.Interfaces;

public interface ITeamsWebhookSender
{
    Task SendAsync(TeamsNotificationMessage message, CancellationToken cancellationToken = default);
}
