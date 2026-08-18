using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.RequestReasons.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/request-reasons")]
public class RequestReasonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestReasonsController(
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
                new GetRequestReasonsQuery(),
                cancellationToken));
    }
}