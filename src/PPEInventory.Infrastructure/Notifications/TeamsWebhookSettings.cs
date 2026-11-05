namespace PPEInventory.Infrastructure.Notifications;

public sealed class TeamsWebhookSettings
{
    public const string SectionName = "TeamsNotifications";

    public bool Enabled { get; set; }
    public string WarehouseWebhookUrl { get; set; } = string.Empty;
    public string AdministratorWebhookUrl { get; set; } = string.Empty;
}
