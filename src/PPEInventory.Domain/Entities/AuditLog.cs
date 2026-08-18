namespace PPEInventory.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? OldValuesJson { get; set; }

    public string? NewValuesJson { get; set; }

    public int PerformedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public User PerformedByUser { get; set; } = null!;
}