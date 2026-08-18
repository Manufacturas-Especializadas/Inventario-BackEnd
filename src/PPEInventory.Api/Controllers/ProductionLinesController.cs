using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.ProductionLines.Commands.Create;
using PPEInventory.Application.Features.ProductionLines.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/production-lines")]
public class ProductionLinesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductionLinesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetProductionLinesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateProductionLineCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
}