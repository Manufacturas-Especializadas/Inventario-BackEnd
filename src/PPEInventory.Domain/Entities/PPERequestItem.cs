namespace PPEInventory.Domain.Entities;

public class PPERequestItem
{
    public int Id { get; set; }

    public int PPERequestId { get; set; }

    public int PPEProductId { get; set; }

    public int Quantity { get; set; }

    public PPERequest PPERequest { get; set; } = null!;

    public PPEProduct PPEProduct { get; set; } = null!;
}