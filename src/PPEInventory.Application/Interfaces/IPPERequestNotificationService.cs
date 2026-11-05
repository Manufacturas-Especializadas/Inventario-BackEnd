using PPEInventory.Application.Common.Notifications;

namespace PPEInventory.Application.Interfaces;

public interface IPPERequestNotificationService
{
    Task NotifyPendingAsync(PPERequestNotificationContext context, CancellationToken cancellationToken = default);
    Task NotifyPendingApprovalAsync(PPERequestNotificationContext context, CancellationToken cancellationToken = default);
}
