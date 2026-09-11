using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.ProductColors.Commands.Create;
using PPEInventory.Application.Features.ProductColors.Commands.SetStatus;
using PPEInventory.Application.Features.ProductColors.Commands.Update;
using PPEInventory.Application.Features.ProductColors.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/colors")]
public class ProductColorsController
    : ControllerBase
{
    private readonly IMediator
        _mediator;

    public ProductColorsController(
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
                new GetProductColorsQuery(),
                cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(
        Policy =
            AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateProductColorCommand command,
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
        UpdateProductColorRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdateProductColorCommand(
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
        SetProductColorStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetProductColorStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }
}