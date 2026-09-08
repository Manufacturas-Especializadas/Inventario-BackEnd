namespace PPEInventory.Application.Features.OrganizationalUnitPPELimits;

public class OrganizationalUnitPPELimitDto
{
    public int Id { get; set; }

    public int OrganizationalUnitId { get; set; }

    public string OrganizationalUnitName { get; set; } = string.Empty;

    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int MaxQuantityPerCycle { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}