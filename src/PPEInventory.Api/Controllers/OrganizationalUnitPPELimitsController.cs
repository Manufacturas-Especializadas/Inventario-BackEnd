using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.OrganizationalUnitPPELimits.Commands.Set;
using PPEInventory.Application.Features.OrganizationalUnitPPELimits.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route(
    "api/organizational-unit-ppe-limits")]
public class OrganizationalUnitPPELimitsController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationalUnitPPELimitsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        [FromQuery]
        int? organizationalUnitId,
        [FromQuery]
        int? ppeProductId,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetOrganizationalUnitPPELimitsQuery(
                    organizationalUnitId,
                    ppeProductId),
                cancellationToken));
    }

    [HttpPut]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Set(
        SetOrganizationalUnitPPELimitCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }
}