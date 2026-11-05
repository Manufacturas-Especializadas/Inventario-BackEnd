using PPEInventory.Application.Common.Notifications;
using System.Text;

namespace PPEInventory.Infrastructure.Notifications;

public sealed class PPERequestTeamsTemplateFactory
{
    public TeamsNotificationMessage CreatePending(PPERequestNotificationContext context) => new(
        TeamsNotificationDestination.Warehouse,
        "Nueva solicitud de EPP",
        CreateBody(context, pendingApproval: false));

    public TeamsNotificationMessage CreatePendingApproval(PPERequestNotificationContext context) => new(
        TeamsNotificationDestination.Administrator,
        "Solicitud especial de EPP por autorizar",
        CreateBody(context, pendingApproval: true));

    private static string CreateBody(PPERequestNotificationContext context, bool pendingApproval)
    {
        var body = new StringBuilder();
        AddSection(body, "Folio", context.Folio);
        AddSection(body, "Empleado", context.EmployeeName);
        AddSection(body, "Nómina", context.EmployeeNumber);
        AddSection(body, "Área / línea", context.RequestedForOrganizationalUnitName);
        AddSection(body, "EPP solicitado", string.Join("\n",
            context.Items.Select(item => $"• {item.ProductName} — {item.Quantity}")));

        if (pendingApproval)
        {
            AddSection(body, "Motivo", context.ReasonName);
            AddSection(body, "Observaciones", context.Notes);
        }

        AddSection(body, "Estado", pendingApproval ? "Pendiente de autorización" : "Pendiente de entrega");
        return body.ToString().TrimEnd();
    }

    private static void AddSection(StringBuilder body, string label, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            body.Append("**").Append(label).Append("**\n\n").Append(value).Append("\n\n");
    }
}
