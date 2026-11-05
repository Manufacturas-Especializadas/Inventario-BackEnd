using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Common.Notifications;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Infrastructure.Notifications;

internal sealed class PPERequestNotificationService(
    INotificationRecipientResolver recipients,
    IEmailSender sender,
    PPERequestEmailTemplateFactory templates,
    ITeamsWebhookSender teamsSender,
    PPERequestTeamsTemplateFactory teamsTemplates,
    IOptions<MicrosoftGraphSettings> graphSettings,
    IOptions<TeamsWebhookSettings> teamsSettings,
    ILogger<PPERequestNotificationService> logger) : IPPERequestNotificationService
{
    public Task NotifyPendingAsync(PPERequestNotificationContext context, CancellationToken cancellationToken = default) =>
        NotifyAsync(context, AppRoles.Warehouse, teamsTemplates.CreatePending, templates.CreatePending, cancellationToken);

    public Task NotifyPendingApprovalAsync(PPERequestNotificationContext context, CancellationToken cancellationToken = default) =>
        NotifyAsync(context, AppRoles.Administrator, teamsTemplates.CreatePendingApproval, templates.CreatePendingApproval, cancellationToken);

    private async Task NotifyAsync(
        PPERequestNotificationContext context,
        string role,
        Func<PPERequestNotificationContext, TeamsNotificationMessage> createTeamsTemplate,
        Func<IReadOnlyCollection<string>, EmailMessage> createEmailTemplate,
        CancellationToken cancellationToken)
    {
        if (teamsSettings.Value.Enabled)
        {
            await SendChannelAsync(context, "Teams", async () =>
            {
                await teamsSender.SendAsync(createTeamsTemplate(context), cancellationToken);
                return true;
            });
        }

        if (graphSettings.Value.Enabled)
        {
            await SendChannelAsync(context, "Email", async () =>
            {
                var emails = await recipients.GetActiveEmailsByRoleAsync(role, cancellationToken);
                if (emails.Count == 0)
                {
                    logger.LogWarning("No active email recipients were found for role {Role}. PPE request {Folio}.",
                        role, context.Folio);
                    return false;
                }

                await sender.SendAsync(createEmailTemplate(emails), cancellationToken);
                return true;
            });
        }
    }

    private async Task SendChannelAsync(PPERequestNotificationContext context, string channel, Func<Task<bool>> send)
    {
        try
        {
            if (await send())
                logger.LogInformation("PPE request notification sent for {RequestId}/{Folio} via {Channel}.",
                    context.RequestId, context.Folio, channel);
        }
        catch (Exception exception)
        {
            // Isolate channel failures and never log external exception messages or inner exceptions.
            logger.LogWarning("Failed to send PPE request notification for {RequestId}/{Folio} via {Channel}. Error type: {ErrorType}.",
                context.RequestId, context.Folio, channel, exception.GetType().Name);
        }
    }
}
