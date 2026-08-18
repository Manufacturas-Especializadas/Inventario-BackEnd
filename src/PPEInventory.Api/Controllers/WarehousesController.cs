using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Warehouses.Commands.Create;
using PPEInventory.Application.Features.Warehouses.Queries.GetAll;
using PPEInventory.Application.Features.PPERequests.Queries.GetPendingByEmployee;

[ApiController]
[Route("api/warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetWarehousesQuery(),
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateWarehouseCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }

    [HttpGet("pending")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
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
}