namespace PPEInventory.Domain.Entities;

public class UnitOfMeasure
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Symbol { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }
}