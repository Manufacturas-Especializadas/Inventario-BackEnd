using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Suppliers.Commands.Create;
using PPEInventory.Application.Features.Suppliers.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(
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
                new GetSuppliersQuery(),
                cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateSupplierCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }
}