namespace PPEInventory.Domain.Entities;

public class PPECategory
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public User? UpdatedByUser { get; set; }

    public ICollection<PPEProduct> Products { get; set; }
        = new List<PPEProduct>();
}