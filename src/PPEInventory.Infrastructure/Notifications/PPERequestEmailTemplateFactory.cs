using System.Net;
using PPEInventory.Application.Common.Notifications;

namespace PPEInventory.Infrastructure.Notifications;

internal sealed class PPERequestEmailTemplateFactory
{
    public EmailMessage CreatePending(IReadOnlyCollection<string> recipients) => Create(
        recipients,
        "Nueva solicitud de EPP pendiente",
        "Se ha registrado una nueva solicitud de EPP que requiere atención de Almacén.",
        "Ingresa al sistema para consultarla.");

    public EmailMessage CreatePendingApproval(IReadOnlyCollection<string> recipients) => Create(
        recipients,
        "Nueva solicitud especial de EPP por autorizar",
        "Se ha registrado una solicitud excepcional de EPP que requiere autorización.",
        "Ingresa al sistema para revisarla.");

    private static EmailMessage Create(
        IReadOnlyCollection<string> recipients, string subject, string message, string action)
    {
        var html = $"""
            <!DOCTYPE html>
            <html lang="es">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"></head>
            <body style="margin:0;padding:0;background-color:#f1f5f9;font-family:Arial,Helvetica,sans-serif;color:#0f172a;">
              <table role="presentation" style="width:100%;border-collapse:collapse;background-color:#f1f5f9;">
                <tr><td align="center" style="padding:24px 12px;">
                  <table role="presentation" style="width:100%;max-width:600px;border-collapse:collapse;background-color:#ffffff;border:1px solid #e2e8f0;">
                    <tr><td style="padding:24px;border-top:4px solid #0284c7;border-bottom:1px solid #e2e8f0;">
                      <p style="margin:0;font-size:24px;font-weight:bold;color:#0369a1;">MESA</p>
                      <p style="margin:8px 0 0;font-size:14px;color:#475569;">Equipo de Protección Personal</p>
                    </td></tr>
                    <tr><td style="padding:24px;">
                      <h1 style="margin:0 0 20px;font-size:22px;line-height:1.4;">{WebUtility.HtmlEncode(subject)}</h1>
                      <p style="margin:0 0 16px;font-size:16px;line-height:1.6;">{WebUtility.HtmlEncode(message)}</p>
                      <p style="margin:0;font-size:16px;line-height:1.6;color:#475569;">{WebUtility.HtmlEncode(action)}</p>
                    </td></tr>
                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
        return new EmailMessage(recipients, subject, html);
    }
}
