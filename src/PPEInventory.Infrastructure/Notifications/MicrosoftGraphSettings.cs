namespace PPEInventory.Infrastructure.Notifications;

public sealed class MicrosoftGraphSettings
{
    public const string SectionName = "MicrosoftGraph";

    public bool Enabled { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SenderUserPrincipalName { get; set; } = string.Empty;
}
