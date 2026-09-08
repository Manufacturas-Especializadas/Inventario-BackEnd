namespace PPEInventory.Domain.Entities;

public class OrganizationalUnitPPELimit
{
    public int Id { get; set; }

    public int OrganizationalUnitId
    {
        get;
        set;
    }

    public int PPEProductId { get; set; }

    public int MaxQuantityPerCycle { get; set; }

    public bool IsActive { get; set; }
        = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public OrganizationalUnit OrganizationalUnit
    {
        get;
        set;
    } = null!;

    public PPEProduct PPEProduct
    {
        get;
        set;
    } = null!;
}