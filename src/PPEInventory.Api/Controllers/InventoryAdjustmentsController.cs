using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.InventoryAdjustments.Commands.Create;
using PPEInventory.Application.Features.InventoryAdjustments.Queries.GetByFolio;
using PPEInventory.Application.Features.InventoryAdjustments.Queries.GetList;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/inventory-adjustments")]
public class InventoryAdjustmentsController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryAdjustmentsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateInventoryAdjustmentCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }

    [HttpGet]
    [Authorize(
    Policy =
        AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> Get(
    [FromQuery] int? warehouseId,
    [FromQuery] DateOnly? dateFrom,
    [FromQuery] DateOnly? dateTo,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetInventoryAdjustmentsQuery(
                    warehouseId,
                    dateFrom,
                    dateTo),
                cancellationToken));
    }

    [HttpGet("{folio}")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetByFolio(
    string folio,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetInventoryAdjustmentByFolioQuery(
                    folio),
                cancellationToken));
    }
}