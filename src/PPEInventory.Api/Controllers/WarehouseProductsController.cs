using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.WarehouseProducts.Commands.ChangeStatus;
using PPEInventory.Application.Features.WarehouseProducts.Commands.Create;
using PPEInventory.Application.Features.WarehouseProducts.Queries.GetByWarehouse;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/warehouse-products")]
public class WarehouseProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseProductsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet("by-warehouse/{warehouseId:int}")]
    [Authorize(
        Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetByWarehouse(
        int warehouseId,
        CancellationToken cancellationToken)
    {
        if (warehouseId <= 0)
        {
            return BadRequest(
                new
                {
                    message = "El almacén no es válido."
                });
        }

        var result =
            await _mediator.Send(
                new GetWarehouseProductsByWarehouseQuery(
                    warehouseId),
                cancellationToken);

        return Ok(result);
    }


    [HttpPost]
    [Authorize(
        Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateWarehouseProductCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }


    [HttpPut("{warehouseId:int}/{ppeProductId:int}/status")]
    [Authorize(
        Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> ChangeStatus(
        int warehouseId,
        int ppeProductId,
        ChangeWarehouseProductStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (warehouseId <= 0 ||
            ppeProductId <= 0)
        {
            return BadRequest(
                new
                {
                    message =
                        "El almacén o producto no es válido."
                });
        }

        return Ok(
            await _mediator.Send(
                new ChangeWarehouseProductStatusCommand(
                    warehouseId,
                    ppeProductId,
                    request.IsActive),
                cancellationToken));
    }
}


public record ChangeWarehouseProductStatusRequest(
    bool IsActive);