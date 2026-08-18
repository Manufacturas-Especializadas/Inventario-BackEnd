using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.GoodsReceipts.Commands.ReceivePurchaseOrder;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/goods-receipts")]
public class GoodsReceiptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GoodsReceiptsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> Receive(
        ReceivePurchaseOrderCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }
}