namespace PPEInventory.Application.Features
    .InventoryCounts.Commands.Cancel;

public class CancelInventoryCountRequest
{
    public string Reason { get; set; } = string.Empty;
}