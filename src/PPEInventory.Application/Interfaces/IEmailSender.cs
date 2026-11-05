using PPEInventory.Application.Common.Notifications;

namespace PPEInventory.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
