using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PPEInventory.Application.Common.Notifications;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Infrastructure.Notifications;

internal sealed class TeamsWebhookSender(
    HttpClient client,
    IOptions<TeamsWebhookSettings> options,
    ILogger<TeamsWebhookSender> logger) : ITeamsWebhookSender
{
    public async Task SendAsync(TeamsNotificationMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(message);

        var settings = options.Value;
        if (!settings.Enabled)
            throw new InvalidOperationException("Teams notifications are disabled (TeamsNotifications:Enabled=false).");

        var (url, settingName) = message.Destination switch
        {
            TeamsNotificationDestination.Warehouse =>
                (settings.WarehouseWebhookUrl, nameof(TeamsWebhookSettings.WarehouseWebhookUrl)),
            TeamsNotificationDestination.Administrator =>
                (settings.AdministratorWebhookUrl, nameof(TeamsWebhookSettings.AdministratorWebhookUrl)),
            _ => throw new ArgumentOutOfRangeException(nameof(message.Destination), "Unsupported Teams destination.")
        };

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException($"TeamsNotifications:{settingName} is required for {message.Destination}.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps
            || !string.IsNullOrEmpty(uri.UserInfo)
            || !string.IsNullOrEmpty(uri.Fragment))
            throw new InvalidOperationException($"TeamsNotifications:{settingName} must be an absolute HTTPS webhook URL without user information or a fragment.");

        var payload = new
        {
            type = "message",
            attachments = new[]
            {
                new
                {
                    contentType = "application/vnd.microsoft.card.adaptive",
                    contentUrl = (string?)null,
                    content = new Dictionary<string, object>
                    {
                        ["$schema"] = "http://adaptivecards.io/schemas/adaptive-card.json",
                        ["type"] = "AdaptiveCard",
                        ["version"] = "1.2",
                        ["body"] = new object[]
                        {
                            new { type = "TextBlock", text = message.Title, weight = "Bolder", size = "Medium", wrap = true },
                            new { type = "TextBlock", text = message.Body, wrap = true }
                        }
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            // JsonContent serializes as UTF-8 with application/json; charset=utf-8.
            Content = JsonContent.Create(payload)
        };

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning("Teams notification transport failed for {Destination}.", message.Destination);
            // Transport exception messages/inner exceptions may contain the secret request URI.
            throw new HttpRequestException(
                $"Teams notification transport failed for {message.Destination}.", null, exception.StatusCode);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Teams notification canceled or timed out for {Destination}.", message.Destination);
            throw new OperationCanceledException(
                $"Teams notification canceled or timed out for {message.Destination}.", cancellationToken);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Teams notification failed for {Destination} with status {StatusCode}.",
                    message.Destination, (int)response.StatusCode);
                // Do not include response bodies/reason phrases: a remote server could echo secrets.
                throw new HttpRequestException(
                    $"Teams notification failed for {message.Destination} with status {(int)response.StatusCode}.",
                    null, response.StatusCode);
            }
        }

        logger.LogInformation("Teams notification sent successfully to {Destination}.", message.Destination);
    }
}
