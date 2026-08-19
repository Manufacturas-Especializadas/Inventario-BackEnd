using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.ProductSuppliers.Commands.Create;
using PPEInventory.Application.Features.ProductSuppliers.Queries.GetAll;
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


    [HttpGet("{ppeProductId:int}")]
    [Authorize(
        Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetByProduct(
        int ppeProductId,
        CancellationToken cancellationToken)
    {
        if (ppeProductId <= 0)
        {
            return BadRequest(
                new
                {
                    message =
                        "El producto EPP no es válido."
                });
        }

        var result =
            await _mediator.Send(
                new GetProductSuppliersQuery(
                    ppeProductId),
                cancellationToken);

        return Ok(result);
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