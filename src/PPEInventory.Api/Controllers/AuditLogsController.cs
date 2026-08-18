using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.AuditLogs.Queries.GetRecent;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/audit-logs")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? entityName = null,
        [FromQuery] int? performedByUserId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        return Ok(
            await _mediator.Send(
                new GetRecentAuditLogsQuery(
                    entityName,
                    performedByUserId,
                    dateFrom,
                    dateTo,
                    pageNumber,
                    pageSize),
                cancellationToken));
    }
}