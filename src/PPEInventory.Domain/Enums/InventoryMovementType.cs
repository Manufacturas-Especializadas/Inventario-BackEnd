namespace PPEInventory.Domain.Enums;

public enum InventoryMovementType
{
    InitialBalance = 1,
    PurchaseReceipt = 2,
    EmployeeIssue = 3,
    AdjustmentIncrease = 4,
    AdjustmentDecrease = 5,
    Return = 6,
    CountAdjustment = 7
}