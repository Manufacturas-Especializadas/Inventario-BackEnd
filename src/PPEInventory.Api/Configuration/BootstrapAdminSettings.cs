namespace PPEInventory.Api.Configuration;

public class BootstrapAdminSettings
{
    public const string SectionName = "BootstrapAdmin";

    public bool Enabled { get; set; }

    public string? Key { get; set; }
}