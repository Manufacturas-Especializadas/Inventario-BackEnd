namespace PPEInventory.Application.Features.Units;

public class UnitOfMeasureDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Symbol { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}