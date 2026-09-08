using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.OrganizationalUnits;

public class OrganizationalUnitDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public OrganizationalUnitType Type { get; set; }

    public int? ParentId { get; set; }

    public string? ParentName { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}