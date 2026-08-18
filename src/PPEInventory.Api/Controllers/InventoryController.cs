using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Inventory.Queries.GetBalances;
using PPEInventory.Application.Features.Inventory.Queries.GetLowStock;
using PPEInventory.Application.Features.Inventory.Queries.GetMovements;
using PPEInventory.Domain.Enums;


namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("balances")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetBalances(
        [FromQuery] int? warehouseId,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetInventoryBalancesQuery(
                    warehouseId),
                cancellationToken));
    }

    [HttpGet("movements")]
    [Authorize(
    Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetMovements(
    [FromQuery] int? warehouseId,
    [FromQuery] int? ppeProductId,
    [FromQuery] InventoryMovementType? movementType,
    [FromQuery] DateTime? dateFrom,
    [FromQuery] DateTime? dateTo,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 25,
    CancellationToken cancellationToken = default)
    {
        return Ok(
            await _mediator.Send(
                new GetInventoryMovementsQuery(
                    warehouseId,
                    ppeProductId,
                    movementType,
                    dateFrom,
                    dateTo,
                    pageNumber,
                    pageSize),
                cancellationToken));
    }

    [HttpGet("low-stock")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetLowStock(
    [FromQuery] int warehouseId,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetLowStockQuery(
                    warehouseId),
                cancellationToken));
    }
}