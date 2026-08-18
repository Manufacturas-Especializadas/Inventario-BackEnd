using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.ProductSuppliers.Commands.Create;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/product-suppliers")]
public class ProductSuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductSuppliersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreateProductSupplierCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _mediator.Send(
                command,
                cancellationToken));
    }
}