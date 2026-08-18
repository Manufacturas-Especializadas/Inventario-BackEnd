using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.PurchaseOrders.Commands.Create;
using PPEInventory.Application.Features.PurchaseOrders.Queries.GetAll;
using PPEInventory.Application.Features.PurchaseOrders.Queries.GetByFolio;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseOrdersController(
        IMediator mediator)
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
                new GetPurchaseOrdersQuery(),
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
                new GetPurchaseOrderByFolioQuery(
                    folio),
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Production)]
    public async Task<IActionResult> Create(
        CreatePurchaseOrderCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }
}