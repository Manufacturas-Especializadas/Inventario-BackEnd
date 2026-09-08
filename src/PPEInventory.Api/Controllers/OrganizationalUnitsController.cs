using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.OrganizationalUnits.Commands.Create;
using PPEInventory.Application.Features.OrganizationalUnits.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/organizational-units")]
public class OrganizationalUnitsController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationalUnitsController(
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
                new GetOrganizationalUnitsQuery(),
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateOrganizationalUnitCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }
}