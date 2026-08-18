using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.PPERequests.Cancellation;
using PPEInventory.Application.Features.PPERequests.Commands.Cancel;
using PPEInventory.Application.Features.PPERequests.Commands.Create;
using PPEInventory.Application.Features.PPERequests.Commands.Deliver;
using PPEInventory.Application.Features.PPERequests.Delivery;
using PPEInventory.Application.Features.PPERequests.Queries.GetByFolio;
using PPEInventory.Application.Features.PPERequests.Queries.GetEmployeeHistory;
using PPEInventory.Application.Features.PPERequests.Queries.GetPending;
using PPEInventory.Application.Features.PPERequests.Queries.GetPendingByEmployee;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/ppe-requests")]
public class PPERequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PPERequestsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{folio}")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetByFolio(
        string folio,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetPPERequestByFolioQuery(
                    folio),
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Production)]
    public async Task<IActionResult> Create(
        CreatePPERequestCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }

    [HttpPost("{folio}/deliver")]
    [Authorize(Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> Deliver(
    string folio,
    DeliverPPERequestRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new DeliverPPERequestCommand(
                    folio,
                    request.EmployeeNumber),
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("{folio}/cancel")]
    [Authorize(Policy = AuthorizationPolicies.Production)]
    public async Task<IActionResult> Cancel(
    string folio,
    CancelPPERequestRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new CancelPPERequestCommand(
                    folio,
                    request.CancellationReason),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("pending/by-employee")]
    [Authorize(
    Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetPendingByEmployee(
    [FromQuery] string employeeNumber,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetPendingPPERequestsByEmployeeQuery(
                    employeeNumber),
                cancellationToken));
    }

    [HttpGet("pending")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetPending(
    [FromQuery] int? warehouseId,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetPendingPPERequestsQuery(
                    warehouseId),
                cancellationToken));
    }

    [HttpGet("history")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetEmployeeHistory(
    [FromQuery] string employeeNumber,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetEmployeePPEHistoryQuery(
                    employeeNumber),
                cancellationToken));
    }
}