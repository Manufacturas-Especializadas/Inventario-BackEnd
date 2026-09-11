using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Units.Commands.Create;
using PPEInventory.Application.Features.Units.Commands.SetStatus;
using PPEInventory.Application.Features.Units.Commands.Update;
using PPEInventory.Application.Features.Units.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetUnitsQuery(),
                cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(
        Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateUnitOfMeasureCommand command,
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
        Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Update(
        int id,
        UpdateUnitOfMeasureRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdateUnitOfMeasureCommand(
                    id,
                    request.Name,
                    request.Symbol),
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(
        Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> SetStatus(
        int id,
        SetUnitOfMeasureStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetUnitOfMeasureStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }
}