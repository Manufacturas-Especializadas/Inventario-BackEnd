namespace PPEInventory.Application.Common.Notifications;

public sealed record EmailMessage(
    IReadOnlyCollection<string> Recipients,
    string Subject,
    string HtmlBody);
