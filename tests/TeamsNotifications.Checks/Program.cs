using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PPEInventory.Application.Common.Notifications;
using PPEInventory.Application.Interfaces;
using PPEInventory.Infrastructure;
using PPEInventory.Infrastructure.Notifications;

// Real delivery is opt-in and exits before the offline checks below.
if (args is ["--send-real"])
{
    Environment.ExitCode = await RealTeamsCheck.RunAsync();
    return;
}

if (args.Length != 0)
{
    Console.Error.WriteLine("Uso: sin argumentos para validación offline; --send-real para un envío real a Warehouse.");
    Environment.ExitCode = 2;
    return;
}

// In-memory transport only. These reserved .invalid addresses never reach a network handler.
const string warehouseUrl = "https://warehouse.invalid/workflow?sig=warehouse-secret";
const string administratorUrl = "https://administrator.invalid/workflow?sig=administrator-secret";
var settings = new TeamsWebhookSettings
{
    Enabled = true,
    WarehouseWebhookUrl = warehouseUrl,
    AdministratorWebhookUrl = administratorUrl
};
var transport = new RecordingHandler();
var logs = new RecordingLoggerProvider();
var services = new ServiceCollection();
services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddProvider(logs));
services.AddInfrastructure("Server=unused;Database=unused");
services.Configure<TeamsWebhookSettings>(value =>
{
    value.Enabled = settings.Enabled;
    value.WarehouseWebhookUrl = settings.WarehouseWebhookUrl;
    value.AdministratorWebhookUrl = settings.AdministratorWebhookUrl;
});
// Replace the typed client's transport before resolving it; no sockets can be created.
services.AddHttpClient(nameof(ITeamsWebhookSender)).ConfigurePrimaryHttpMessageHandler(() => transport);
using var provider = services.BuildServiceProvider();
var sender = provider.GetRequiredService<ITeamsWebhookSender>();
var templates = provider.GetRequiredService<PPERequestTeamsTemplateFactory>();
var actualSettings = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<TeamsWebhookSettings>>().Value;
Check(transport.Calls == 0, "Resolving services must not send notifications.");
Check(provider.GetRequiredService<IEmailSender>() is not null, "Graph remains registered.");

var context = new PPERequestNotificationContext(123, "PPE-000123", "Héctor Camacho", "7162", "L-8",
    "DAMAGE", "Daño", "Los guantes se dañaron durante la operación.",
    new[] { new PPERequestNotificationItem("Guantes anticorte", 2) });
var pending = templates.CreatePending(context);
var approval = templates.CreatePendingApproval(context);
Check(pending.Destination == TeamsNotificationDestination.Warehouse
    && pending.Title == "Nueva solicitud de EPP"
    && pending.Body.Contains("Pendiente de entrega"), "Warehouse template");
Check(approval.Destination == TeamsNotificationDestination.Administrator
    && approval.Title == "Solicitud especial de EPP por autorizar"
    && approval.Body.Contains("Pendiente de autorización"), "Administrator template");

await sender.SendAsync(pending);
ValidatePayload(pending, warehouseUrl);
await sender.SendAsync(approval);
ValidatePayload(approval, administratorUrl);
// Quotes, newlines and non-ASCII characters must survive actual sender serialization.
var unicode = pending with { Title = "atención Almacén autorización", Body = "solicitud \"especial\"\n<texto> ñ 😀" };
await sender.SendAsync(unicode);
ValidatePayload(unicode, warehouseUrl);

foreach (var status in new[] { 200, 201, 202, 204, 299 })
{
    transport.Status = (HttpStatusCode)status;
    await sender.SendAsync(pending);
}
foreach (var status in new[] { 300, 400, 401, 429, 500 })
{
    transport.Status = (HttpStatusCode)status;
    var error = await Expect<HttpRequestException>(() => sender.SendAsync(approval));
    Check(error.StatusCode == (HttpStatusCode)status && error.Message.Contains(status.ToString()), "HTTP error status");
    CheckSafe(error);
}

transport.Failure = new HttpRequestException($"Secret URI: {warehouseUrl}", new Exception(warehouseUrl), HttpStatusCode.BadGateway);
var transportError = await Expect<HttpRequestException>(() => sender.SendAsync(pending));
Check(transportError.StatusCode == HttpStatusCode.BadGateway, "Transport status preserved");
CheckSafe(transportError);
transport.Failure = new TaskCanceledException(warehouseUrl, new Exception(warehouseUrl));
CheckSafe(await Expect<OperationCanceledException>(() => sender.SendAsync(pending)));
transport.Failure = null;

var callsBeforeValidation = transport.Calls;
actualSettings.Enabled = false;
var disabled = await Expect<InvalidOperationException>(() => sender.SendAsync(pending));
Check(disabled.Message.Contains("TeamsNotifications:Enabled=false"), "Disabled configuration error");
actualSettings.Enabled = true;
actualSettings.WarehouseWebhookUrl = "";
var missingWarehouse = await Expect<InvalidOperationException>(() => sender.SendAsync(pending));
Check(missingWarehouse.Message.Contains("TeamsNotifications:WarehouseWebhookUrl"), "Missing warehouse setting");
actualSettings.AdministratorWebhookUrl = " ";
var missingAdministrator = await Expect<InvalidOperationException>(() => sender.SendAsync(approval));
Check(missingAdministrator.Message.Contains("TeamsNotifications:AdministratorWebhookUrl"), "Missing administrator setting");
foreach (var invalid in new[] { "invalid-secret", "http://warehouse.invalid/?sig=secret", warehouseUrl + "#fragment" })
{
    actualSettings.WarehouseWebhookUrl = invalid;
    CheckSafe(await Expect<InvalidOperationException>(() => sender.SendAsync(pending)));
}
await Expect<ArgumentOutOfRangeException>(() => sender.SendAsync(pending with { Destination = (TeamsNotificationDestination)99 }));
using var cancellation = new CancellationTokenSource();
cancellation.Cancel();
var canceled = await Expect<OperationCanceledException>(() => sender.SendAsync(pending, cancellation.Token));
Check(canceled.CancellationToken == cancellation.Token, "Cancellation token preserved");
Check(transport.Calls == callsBeforeValidation, "Configuration errors/cancellation must not reach transport.");

actualSettings.WarehouseWebhookUrl = warehouseUrl;
transport.Failure = null;
transport.Status = HttpStatusCode.Accepted;
transport.WaitForCancellation = true;
using var duringSend = new CancellationTokenSource();
var sending = sender.SendAsync(pending, duringSend.Token);
duringSend.Cancel();
var canceledDuringSend = await Expect<OperationCanceledException>(() => sending);
Check(canceledDuringSend.CancellationToken == duringSend.Token, "In-flight cancellation propagated");
CheckSafe(canceledDuringSend);

Check(logs.Messages.Any(x => x.Contains("sent successfully to Warehouse")), "Success logging");
Check(logs.Messages.Any(x => x.Contains("Administrator with status 400")), "Failure logging");
Check(logs.Messages.All(x => !x.Contains("https://") && !x.Contains("sig=") && !x.Contains("secret")), "No URLs/secrets in logs, including factory HTTP logs");
await PPERequestNotificationChecks.RunAsync();
Console.WriteLine("PASS: DI, templates, exact Adaptive Card structure, UTF-8 accents, destinations, 2xx, HTTP/transport errors, safe logs, configuration and cancellation. No network connections.");

void ValidatePayload(TeamsNotificationMessage expected, string expectedUri)
{
    Check(transport.Method == HttpMethod.Post && transport.Uri == expectedUri, "POST and destination");
    Check(transport.ContentType == "application/json; charset=utf-8", "UTF-8 content type");
    var json = new UTF8Encoding(false, true).GetString(transport.Bytes!);
    foreach (var required in new[] { "\"type\":\"message\"", "\"contentType\":\"application/vnd.microsoft.card.adaptive\"", "\"type\":\"AdaptiveCard\"", "\"version\":\"1.2\"" })
        Check(json.Contains(required), "Required JSON fragment: " + required);
    using var document = JsonDocument.Parse(json);
    var root = document.RootElement;
    Check(root.EnumerateObject().Count() == 2, "Envelope properties");
    var attachments = root.GetProperty("attachments");
    Check(attachments.GetArrayLength() == 1, "Single attachment");
    var attachment = attachments[0];
    Check(attachment.EnumerateObject().Count() == 3 && attachment.GetProperty("contentUrl").ValueKind == JsonValueKind.Null, "Explicit null contentUrl");
    var card = attachment.GetProperty("content");
    Check(card.EnumerateObject().Count() == 4 && card.GetProperty("$schema").GetString() == "http://adaptivecards.io/schemas/adaptive-card.json", "Card schema");
    var body = card.GetProperty("body");
    Check(body.GetArrayLength() == 2, "Exactly two text blocks");
    Check(body[0].EnumerateObject().Count() == 5 && body[1].EnumerateObject().Count() == 3, "No extra text block fields");
    Check(body[0].GetProperty("text").GetString() == expected.Title && body[1].GetProperty("text").GetString() == expected.Body, "Unicode text round-trip");
    Check(body[0].GetProperty("weight").GetString() == "Bolder" && body[0].GetProperty("size").GetString() == "Medium", "Title formatting");
    foreach (var block in body.EnumerateArray())
        Check(block.GetProperty("type").GetString() == "TextBlock" && block.GetProperty("wrap").GetBoolean(), "Wrapped TextBlock");
}

static void CheckSafe(Exception error) => Check(
    !error.ToString().Contains("https://") && !error.ToString().Contains("secret") && error.InnerException is null,
    "Exception must not expose webhook secrets");

static void Check(bool condition, string description)
{
    if (!condition) throw new Exception("FAIL: " + description);
}

static async Task<T> Expect<T>(Func<Task> action) where T : Exception
{
    try { await action(); }
    catch (T exception) { return exception; }
    throw new Exception("Expected " + typeof(T).Name);
}

sealed class RecordingHandler : HttpMessageHandler
{
    public int Calls { get; private set; }
    public string? Uri { get; private set; }
    public HttpMethod? Method { get; private set; }
    public string? ContentType { get; private set; }
    public byte[]? Bytes { get; private set; }
    public HttpStatusCode Status { get; set; } = HttpStatusCode.Accepted;
    public Exception? Failure { get; set; }
    public bool WaitForCancellation { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        Uri = request.RequestUri!.AbsoluteUri;
        Method = request.Method;
        ContentType = request.Content!.Headers.ContentType!.ToString();
        Bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
        if (WaitForCancellation) await Task.Delay(Timeout.Infinite, cancellationToken);
        if (Failure is not null) throw Failure;
        return new HttpResponseMessage(Status)
        {
            Content = new StringContent("Never expose this response secret: " + Uri),
            ReasonPhrase = "Never expose this reason secret"
        };
    }
}

sealed class RecordingLoggerProvider : ILoggerProvider
{
    public List<string> Messages { get; } = [];
    public ILogger CreateLogger(string categoryName) => new RecordingLogger(Messages);
    public void Dispose() { }

    private sealed class RecordingLogger(List<string> messages) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => messages.Add(formatter(state, exception));
    }
}
