namespace PPEInventory.Application.Features.WarehouseProducts.Commands.BulkAssign;

public class BulkAssignWarehouseProductsResultDto
{
    public int CreatedCount { get; set; }

    public int ReactivatedCount { get; set; }

    public int AlreadyActiveCount { get; set; }

    public int TotalProcessed { get; set; }
}