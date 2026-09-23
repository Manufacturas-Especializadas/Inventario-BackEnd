using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.ProductSizes.Commands.Create;
using PPEInventory.Application.Features.ProductSizes.Commands.SetStatus;
using PPEInventory.Application.Features.ProductSizes.Commands.Update;
using PPEInventory.Application.Features.ProductSizes.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/sizes")]
public class ProductSizesController
    : ControllerBase
{
    private readonly IMediator
        _mediator;

    public ProductSizesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetProductSizesQuery(),
                cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(
        Policy =
            AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateProductSizeCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                command,
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(
    Policy =
        AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Update(
    int id,
    UpdateProductSizeRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdateProductSizeCommand(
                    id,
                    request.Name),
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(
        Policy =
            AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> SetStatus(
        int id,
        SetProductSizeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetProductSizeStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }
}