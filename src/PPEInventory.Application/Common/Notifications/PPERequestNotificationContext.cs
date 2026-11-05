namespace PPEInventory.Application.Common.Notifications;

public sealed record PPERequestNotificationContext(
    int RequestId,
    string Folio,
    string EmployeeName,
    string EmployeeNumber,
    string? RequestedForOrganizationalUnitName,
    string ReasonCode,
    string ReasonName,
    string? Notes,
    IReadOnlyCollection<PPERequestNotificationItem> Items);

public sealed record PPERequestNotificationItem(string ProductName, int Quantity);
