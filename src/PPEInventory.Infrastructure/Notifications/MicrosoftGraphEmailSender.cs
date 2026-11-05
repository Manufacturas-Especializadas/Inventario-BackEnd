using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using PPEInventory.Application.Common.Notifications;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Infrastructure.Notifications;

internal sealed class MicrosoftGraphEmailSender : IEmailSender, IDisposable
{
    private readonly MicrosoftGraphSettings _settings;
    private readonly Lazy<GraphServiceClient> _client;

    public MicrosoftGraphEmailSender(IOptions<MicrosoftGraphSettings> options)
    {
        _settings = options.Value;
        // No credential or client is created during service resolution/startup.
        _client = new Lazy<GraphServiceClient>(() => new GraphServiceClient(
            new ClientSecretCredential(_settings.TenantId, _settings.ClientId, _settings.ClientSecret),
            new[] { "https://graph.microsoft.com/.default" }));
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_settings.Enabled)
            throw new InvalidOperationException("Microsoft Graph email is disabled (MicrosoftGraph:Enabled=false).");

        if (string.IsNullOrWhiteSpace(_settings.TenantId)
            || string.IsNullOrWhiteSpace(_settings.ClientId)
            || string.IsNullOrWhiteSpace(_settings.ClientSecret)
            || string.IsNullOrWhiteSpace(_settings.SenderUserPrincipalName))
            throw new InvalidOperationException(
                "Microsoft Graph email requires TenantId, ClientId, ClientSecret and SenderUserPrincipalName configuration.");

        if (message.Recipients.Count == 0)
            throw new ArgumentException("At least one email recipient is required.", nameof(message));

        var body = new SendMailPostRequestBody
        {
            Message = new Message
            {
                Subject = message.Subject,
                Body = new ItemBody { ContentType = BodyType.Html, Content = message.HtmlBody },
                ToRecipients = message.Recipients.Select(email => new Recipient
                {
                    EmailAddress = new EmailAddress { Address = email }
                }).ToList()
            },
            SaveToSentItems = true
        };

        // Graph/authentication failures propagate to the caller.
        await _client.Value.Users[_settings.SenderUserPrincipalName].SendMail.PostAsync(
            body, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        if (_client.IsValueCreated) _client.Value.Dispose();
    }
}
