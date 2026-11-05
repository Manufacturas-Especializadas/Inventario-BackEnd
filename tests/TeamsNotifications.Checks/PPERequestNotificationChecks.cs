using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PPEInventory.Application.Common.Constants;
using PPEInventory.Application.Common.Notifications;
using PPEInventory.Application.Features.PPERequests;
using PPEInventory.Application.Features.PPERequests.Commands.Create;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;
using PPEInventory.Infrastructure;
using PPEInventory.Infrastructure.Notifications;

internal static class PPERequestNotificationChecks
{
    public static async Task RunAsync()
    {
        foreach (var exceptional in new[] { false, true })
        {
            using var fixture = new Fixture(exceptional);
            var result = await fixture.CreateAsync();
            var expectedStatus = exceptional ? PPERequestStatus.PendingApproval : PPERequestStatus.Pending;
            var expectedDestination = exceptional ? TeamsNotificationDestination.Administrator : TeamsNotificationDestination.Warehouse;
            Check(result.Request.Status == expectedStatus, "Created status");
            Check(fixture.Messages.Count == 1 && fixture.Messages[0].Destination == expectedDestination, "Exactly one correct destination");
            Check(fixture.Events.SequenceEqual(new[] { "Save", "Commit", "Teams" }), "Save -> Commit -> notification");
            Check(fixture.Transaction.Rollbacks == 0 && fixture.Transaction.Committed, "Successful commit without rollback");
            Check(fixture.Balance.ReservedQuantity == (exceptional ? 0 : 2), "Existing reservation rules unchanged");
            Check(fixture.EmailCalls == 0 && fixture.RecipientCalls == 0, "Disabled Graph never resolves recipients or sends email");
            var card = fixture.Messages[0];
            foreach (var text in new[] { "Folio", "PPE-000123", "Empleado", "Héctor Camacho Corona", "Nómina", "7162", "Área / línea", "L-8", "EPP solicitado", "• Guantes anticorte — 2", "Estado" })
                Check(card.Body.Contains(text), "Card contains " + text);
            if (exceptional)
            {
                foreach (var text in new[] { "Motivo", "Daño", "Observaciones", "Los guantes se dañaron durante la operación.", "Pendiente de autorización" })
                    Check(card.Body.Contains(text), "Exceptional card contains " + text);
            }
            else
                Check(card.Body.Contains("Pendiente de entrega") && !card.Body.Contains("Observaciones") && !card.Body.Contains("Motivo"), "Normal card sections");
            Check(fixture.Logs.Messages.Any(x => x.Contains("notification sent for 123/PPE-000123")), "Success log with ID/folio");
        }

        foreach (var failure in new Exception[]
        {
            new HttpRequestException("https://secret.invalid/?token=DO_NOT_LOG", null, System.Net.HttpStatusCode.BadRequest),
            new HttpRequestException("DO_NOT_LOG", null, System.Net.HttpStatusCode.InternalServerError),
            new HttpRequestException("DO_NOT_LOG"),
            new TaskCanceledException("DO_NOT_LOG"),
            new InvalidOperationException("DO_NOT_LOG")
        })
        {
            using var fixture = new Fixture(exceptional: false) { TeamsFailure = failure };
            var result = await fixture.CreateAsync();
            Check(result.Request.Id == 123 && result.Request.Status == PPERequestStatus.Pending, "Sender error preserves creation result");
            Check(fixture.Transaction.Committed && fixture.Transaction.Rollbacks == 0, "Sender error cannot undo commit");
            Check(fixture.Events.SequenceEqual(new[] { "Save", "Commit", "Teams" }), "Single send attempt after commit on failure");
            Check(fixture.Balance.ReservedQuantity == 2, "Sender failure preserves reservation");
            Check(fixture.Logs.Messages.Any(x => x.Contains("Failed to send PPE request notification for 123/PPE-000123")), "Failure log with ID/folio");
            Check(fixture.Logs.Messages.All(x => !x.Contains("DO_NOT_LOG") && !x.Contains("https://")), "No secret exception details in logs");
        }

        // Exercise the handler's own protection independently of the orchestrator's protection.
        using (var fixture = new Fixture(exceptional: true))
        {
            var throwingService = Stub<IPPERequestNotificationService>((method, _) =>
            {
                fixture.Events.Add("Notification");
                Check(method == nameof(IPPERequestNotificationService.NotifyPendingApprovalAsync), "Exceptional notification method");
                throw new Exception("DO_NOT_LOG");
            });
            var result = await fixture.CreateAsync(throwingService);
            Check(result.Request.Status == PPERequestStatus.PendingApproval && fixture.Transaction.Rollbacks == 0, "Handler isolates arbitrary notification failure");
            Check(fixture.Events.SequenceEqual(new[] { "Save", "Commit", "Notification" }), "Handler calls business abstraction after commit");
            Check(fixture.Logs.Messages.Any(x => x.Contains("Failed to send PPE request notification"))
                && fixture.Logs.Messages.All(x => !x.Contains("DO_NOT_LOG")), "Handler failure log is safe");
        }

        foreach (var failSave in new[] { false, true })
        {
            using var fixture = new Fixture(exceptional: false) { FailSave = failSave };
            fixture.Transaction.FailCommit = !failSave;
            var failed = false;
            try { await fixture.CreateAsync(); }
            catch (InvalidOperationException) { failed = true; }
            Check(failed && fixture.Transaction.Rollbacks == 1 && !fixture.Transaction.Committed, "Persistence failure rolls back");
            Check(fixture.Messages.Count == 0 && fixture.EmailCalls == 0, "No notification before successful commit");
        }

        using (var fixture = new Fixture(exceptional: false) { FinalStatus = PPERequestStatus.PendingApproval })
        {
            await fixture.CreateAsync();
            Check(fixture.Messages.Single().Destination == TeamsNotificationDestination.Administrator, "Routing uses final status, not reason code");
        }

        foreach (var teamsEnabled in new[] { false, true })
        foreach (var emailEnabled in new[] { false, true })
        {
            using var fixture = new Fixture(exceptional: true, teamsEnabled, emailEnabled);
            await fixture.CreateAsync();
            Check(fixture.Messages.Count == (teamsEnabled ? 1 : 0), "Teams enable flag");
            Check(fixture.EmailCalls == (emailEnabled ? 1 : 0) && fixture.RecipientCalls == (emailEnabled ? 1 : 0), "Graph enable flag");
            if (emailEnabled) Check(fixture.RecipientRole == AppRoles.Administrator, "Email role routing preserved");
        }

        using (var fixture = new Fixture(exceptional: false, teamsEnabled: true, emailEnabled: true)
        { TeamsFailure = new Exception("DO_NOT_LOG") })
        {
            await fixture.CreateAsync();
            Check(fixture.EmailCalls == 1 && fixture.Messages.Count == 1, "Teams failure does not suppress email");
            Check(fixture.RecipientRole == AppRoles.Warehouse, "Normal email role");
        }

        var templates = new PPERequestTeamsTemplateFactory();
        var context = new PPERequestNotificationContext(1, "PPE-000001", "Empleado", "1", null, "DAMAGE", "Daño", null,
            new[] { new PPERequestNotificationItem("Casco", 1), new PPERequestNotificationItem("Lentes", 3) });
        foreach (var notes in new string?[] { null, "", "  " })
        {
            var message = templates.CreatePendingApproval(context with { Notes = notes });
            Check(!message.Body.Contains("Observaciones") && !message.Body.Contains("Área / línea"), "Omit absent optional data");
            Check(message.Body.Contains("• Casco — 1\n• Lentes — 3"), "All items and quantities");
        }

        Console.WriteLine("PASS: real creation handler + channel orchestrator (offline), post-commit ordering, status routing, card content, channel flags, failure isolation and reservation behavior.");
    }

    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception("FAIL: " + description);
    }

    private static T Stub<T>(Func<string, object?[], object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, RepositoryStub>();
        ((RepositoryStub)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }

    private sealed class Fixture : IDisposable
    {
        public List<string> Events { get; } = [];
        public List<TeamsNotificationMessage> Messages { get; } = [];
        public RecordingLoggerProvider Logs { get; } = new();
        public Transaction Transaction { get; }
        public InventoryBalance Balance { get; } = new() { PPEProductId = 1, WarehouseId = 1, OnHandQuantity = 20 };
        public Exception? TeamsFailure { get; init; }
        public bool FailSave { get; init; }
        public PPERequestStatus? FinalStatus { get; init; }
        public int EmailCalls { get; private set; }
        public int RecipientCalls { get; private set; }
        public string? RecipientRole { get; private set; }
        private readonly bool _exceptional;
        private readonly ServiceProvider _provider;
        private PPERequest? _saved;

        public Fixture(bool exceptional, bool teamsEnabled = true, bool emailEnabled = false)
        {
            _exceptional = exceptional;
            Transaction = new Transaction(Events);
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddProvider(Logs));
            services.AddInfrastructure("Server=unused;Database=unused");
            services.Configure<TeamsWebhookSettings>(settings => settings.Enabled = teamsEnabled);
            services.Configure<MicrosoftGraphSettings>(settings => settings.Enabled = emailEnabled);
            // Replace all outbound dependencies. No real transport or database is used.
            services.AddSingleton(Stub<ITeamsWebhookSender>((_, args) =>
            {
                Events.Add("Teams");
                Messages.Add((TeamsNotificationMessage)args[0]!);
                return TeamsFailure is null ? Task.CompletedTask : Task.FromException(TeamsFailure);
            }));
            services.AddSingleton(Stub<IEmailSender>((_, _) => { EmailCalls++; return Task.CompletedTask; }));
            services.AddSingleton(Stub<INotificationRecipientResolver>((_, args) =>
            {
                RecipientCalls++;
                RecipientRole = (string)args[0]!;
                return Task.FromResult<IReadOnlyCollection<string>>(new[] { "recipient@example.invalid" });
            }));
            _provider = services.BuildServiceProvider();
        }

        public Task<CreatePPERequestResultDto> CreateAsync(IPPERequestNotificationService? notifications = null)
        {
            var employee = new Employee { Id = 1, EmployeeNumber = "7162", Name = "Héctor Camacho Corona", IsActive = true };
            var unit = new OrganizationalUnit { Id = 1, Name = "L-8", IsActive = true };
            var product = new PPEProduct { Id = 1, Name = "Guantes anticorte", IsActive = true,
                StockUnitOfMeasure = new UnitOfMeasure { Id = 1, Name = "Pieza", Symbol = "pza" } };
            var reason = new RequestReason { Id = 1, IsActive = true,
                Code = _exceptional ? RequestReasonCodes.Damage : RequestReasonCodes.InitialAssignment,
                Name = _exceptional ? "Daño" : "Asignación inicial" };
            var requestRepository = Stub<IPPERequestRepository>((method, args) => method switch
            {
                nameof(IPPERequestRepository.AddAsync) => AddRequest((PPERequest)args[0]!),
                nameof(IPPERequestRepository.GetCommittedNormalQuantitiesInCycleAsync) => Task.FromResult<IReadOnlyDictionary<int, int>>(new Dictionary<int, int>()),
                _ => throw new InvalidOperationException("Unexpected request repository query")
            });
            var handler = new CreatePPERequestCommandHandler(
                Stub<IEmployeeRepository>((_, _) => Task.FromResult<Employee?>(employee)),
                Stub<IWarehouseRepository>((_, _) => Task.FromResult<Warehouse?>(new Warehouse { Id = 1, Name = "Almacén", IsActive = true })),
                Stub<IRequestReasonRepository>((_, _) => Task.FromResult<RequestReason?>(reason)),
                Stub<IPPEProductRepository>((method, _) => method == nameof(IPPEProductRepository.GetByIdsAsync)
                    ? Task.FromResult<IReadOnlyList<PPEProduct>>(new[] { product }) : throw new InvalidOperationException("Unexpected product query")),
                requestRepository,
                Stub<IInventoryRepository>((_, _) => Task.FromResult<IReadOnlyList<InventoryBalance>>(new[] { Balance })),
                Stub<IInventoryCountRepository>((_, _) => Task.FromResult(false)),
                Stub<IUnitOfWork>((method, _) => method == nameof(IUnitOfWork.BeginSerializableTransactionAsync)
                    ? Task.FromResult<IAppTransaction>(Transaction) : Save()),
                Stub<ICurrentUserService>((method, args) => method == "get_UserId" ? (int?)1 : (object)((string)args[0]! == AppRoles.Administrator)),
                Stub<IDateTimeProvider>((_, _) => new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc)),
                Stub<IOrganizationalUnitPPELimitRepository>((_, _) => Task.FromResult<IReadOnlyList<OrganizationalUnitPPELimit>>(Array.Empty<OrganizationalUnitPPELimit>())),
                Stub<IOrganizationalUnitRepository>((method, _) => method == nameof(IOrganizationalUnitRepository.GetByIdAsync)
                    ? Task.FromResult<OrganizationalUnit?>(unit) : Task.FromResult<IReadOnlyList<OrganizationalUnit>>(new[] { unit })),
                Stub<IWarehouseProductRepository>((_, _) => Task.FromResult<IReadOnlyList<WarehouseProduct>>(new[] { new WarehouseProduct { PPEProductId = 1, WarehouseId = 1, IsActive = true } })),
                Stub<IUserRepository>((_, _) => throw new InvalidOperationException("Unexpected user query")),
                notifications ?? _provider.GetRequiredService<IPPERequestNotificationService>(),
                _provider.GetRequiredService<ILogger<CreatePPERequestCommandHandler>>());
            return handler.Handle(new CreatePPERequestCommand("7162", 1, 1, 1,
                _exceptional ? "Los guantes se dañaron durante la operación." : null,
                new[] { new CreatePPERequestItemRequest(1, 2) }), CancellationToken.None);
        }

        private Task AddRequest(PPERequest request) { _saved = request; return Task.CompletedTask; }
        private Task<int> Save()
        {
            Events.Add("Save");
            if (FailSave) throw new InvalidOperationException("Simulated persistence failure");
            _saved!.Id = 123;
            typeof(PPERequest).GetProperty(nameof(PPERequest.Folio))!.SetValue(_saved, "PPE-000123");
            if (FinalStatus.HasValue) _saved.Status = FinalStatus.Value;
            return Task.FromResult(1);
        }

        public void Dispose() => _provider.Dispose();
    }

    private sealed class Transaction(List<string> events) : IAppTransaction
    {
        public bool Committed { get; private set; }
        public bool FailCommit { get; set; }
        public int Rollbacks { get; private set; }
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (FailCommit) throw new InvalidOperationException("Simulated commit failure");
            Committed = true;
            events.Add("Commit");
            return Task.CompletedTask;
        }
        public Task RollbackAsync(CancellationToken cancellationToken = default) { Rollbacks++; return Task.CompletedTask; }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

public class RepositoryStub : DispatchProxy
{
    public Func<string, object?[], object?> InvokeMethod { get; set; } = null!;
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => InvokeMethod(targetMethod!.Name, args ?? []);
}
