using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Common.Models;
using PPEInventory.Application.Features.PurchaseOrders.Commands.Cancel;
using PPEInventory.Application.Features.PurchaseOrders.Commands.Create;
using PPEInventory.Application.Features.PurchaseOrders.Commands.Update;
using PPEInventory.Application.Features.PurchaseOrders.Queries.GetAll;
using PPEInventory.Application.Features.PurchaseOrders.Queries.GetByFolio;
using PPEInventory.Application.Features.PurchaseOrders.Queries.GetReceivingWarehouses;
using PPEInventory.Domain.Enums;

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
    PurchaseOrderStatus? status = null,
    int pageNumber = 1,
    int pageSize = PaginationParameters.DefaultPageSize,
    CancellationToken cancellationToken = default)
    {
        return Ok(
            await _mediator.Send(
                new GetPurchaseOrdersQuery(
                    status,
                    pageNumber,
                    pageSize),
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

    [HttpGet("{folio}/receiving-warehouses")]
    [Authorize(
    Policy =
        AuthorizationPolicies.Viewer)]
    public async Task<IActionResult>
    GetReceivingWarehouses(
        string folio,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetReceivingWarehousesQuery(
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

    [HttpPut("{folio}")]
    [Authorize(
    Policy =
        AuthorizationPolicies.Production)]
    public async Task<IActionResult> Update(
    string folio,
    UpdatePurchaseOrderRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdatePurchaseOrderCommand(
                    folio,
                    request.SupplierId,
                    request.PurchaseOrderNumber,
                    request.ConfirmedDeliveryDate,
                    request.CurrencyCode,
                    request.Notes,
                    request.Items),
                cancellationToken);

        return Ok(result);
    }


    [HttpPut("{folio}/cancel")]
    [Authorize(
        Policy =
            AuthorizationPolicies.Production)]
    public async Task<IActionResult> Cancel(
        string folio,
        CancelPurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new CancelPurchaseOrderCommand(
                    folio,
                    request.Reason),
                cancellationToken);

        return Ok(result);
    }
}