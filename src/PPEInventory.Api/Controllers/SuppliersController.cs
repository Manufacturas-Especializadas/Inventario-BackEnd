using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Suppliers.Commands.Create;
using PPEInventory.Application.Features.Suppliers.Commands.SetStatus;
using PPEInventory.Application.Features.Suppliers.Commands.Update;
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

    [HttpPut("{id:int}")]
    [Authorize(
    Policy =
        AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Update(
    int id,
    UpdateSupplierRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdateSupplierCommand(
                    id,
                    request.Name,
                    request.ContactName,
                    request.Email,
                    request.Phone),
                cancellationToken);

        return Ok(result);
    }


    [HttpPut("{id:int}/status")]
    [Authorize(
        Policy =
            AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> SetStatus(
        int id,
        SetSupplierStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetSupplierStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }
}