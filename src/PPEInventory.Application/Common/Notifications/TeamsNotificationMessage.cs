namespace PPEInventory.Application.Common.Notifications;

public sealed record TeamsNotificationMessage(
    TeamsNotificationDestination Destination,
    string Title,
    string Body);
