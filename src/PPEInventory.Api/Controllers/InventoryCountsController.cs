using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.InventoryCounts.Commands.Cancel;
using PPEInventory.Application.Features.InventoryCounts.Commands.CaptureItem;
using PPEInventory.Application.Features.InventoryCounts.Commands.DeleteDraft;
using PPEInventory.Application.Features.InventoryCounts.Commands.Post;
using PPEInventory.Application.Features.InventoryCounts.Commands.Start;
using PPEInventory.Application.Features.InventoryCounts.Commands.Submit;
using PPEInventory.Application.Features.InventoryCounts.Queries.GetByFolio;
using PPEInventory.Application.Features.InventoryCounts.Queries.GetDrafts;
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

    [HttpGet("drafts")]
    [Authorize(
    Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> GetDrafts(
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetDraftInventoryCountsQuery(),
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
    [HttpPost("{folio}/cancel")]
    [Authorize(
    Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Cancel(
    string folio,
    CancelInventoryCountRequest request,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new CancelInventoryCountCommand(
                    folio,
                    request.Reason),
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

    [HttpDelete("{folio}")]
    [Authorize(
    Policy = AuthorizationPolicies.Warehouse)]
    public async Task<IActionResult> DeleteDraft(
    string folio,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteDraftInventoryCountCommand(
                folio),
            cancellationToken);

        return NoContent();
    }
}