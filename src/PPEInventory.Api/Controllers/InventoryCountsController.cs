using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.InventoryCounts.Commands.CaptureItem;
using PPEInventory.Application.Features.InventoryCounts.Commands.Post;
using PPEInventory.Application.Features.InventoryCounts.Commands.Start;
using PPEInventory.Application.Features.InventoryCounts.Commands.Submit;
using PPEInventory.Application.Features.InventoryCounts.Queries.GetByFolio;
using PPEInventory.Application.Features.InventoryCounts.Queries.GetPendingReview;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/inventory-counts")]
public class InventoryCountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryCountsController(
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
                new GetInventoryCountByFolioQuery(
                    folio),
                cancellationToken));
    }

    [HttpGet("pending-review")]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> GetPendingReview(
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetPendingReviewInventoryCountsQuery(),
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> Start(
        StartInventoryCountCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }

    [HttpPut("{folio}/items/{ppeProductId:int}")]
    [Authorize(Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> CaptureItem(
        string folio,
        int ppeProductId,
        CaptureInventoryCountItemRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new CaptureInventoryCountItemCommand(
                    folio,
                    ppeProductId,
                    request.CountedQuantity),
                cancellationToken));
    }

    [HttpPost("{folio}/submit")]
    [Authorize(Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> Submit(
        string folio,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new SubmitInventoryCountCommand(
                    folio),
                cancellationToken));
    }

    [HttpPost("{folio}/post")]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Post(
        string folio,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new PostInventoryCountCommand(
                    folio),
                cancellationToken));
    }
}