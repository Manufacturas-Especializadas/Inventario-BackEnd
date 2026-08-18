using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.PPEProducts.Commands.Create;
using PPEInventory.Application.Features.PPEProducts.Queries.GetAll;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/ppe-products")]
public class PPEProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PPEProductsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPPEProductsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreatePPEProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{productId:int}/suppliers")]
    [Authorize(
    Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetSuppliers(
    int productId,
    CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                new GetProductSuppliersQuery(productId),
                cancellationToken));
    }

}