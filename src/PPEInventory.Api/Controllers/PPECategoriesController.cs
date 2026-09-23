using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.PPECategories.Commands.Create;
using PPEInventory.Application.Features.PPECategories.Queries.GetAll;
using PPEInventory.Application.Features.PPECategories.Commands.SetStatus;
using PPEInventory.Application.Features.PPECategories.Commands.Update;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/ppe-categories")]
public class PPECategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PPECategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPPECategoriesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Create(
        CreatePPECategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> Update(
    int id,
    UpdatePPECategoryRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdatePPECategoryCommand(
                    id,
                    request.Name,
                    request.Description),
                cancellationToken);

        return Ok(result);
    }


    [HttpPut("{id:int}/status")]
    [Authorize(Policy = AuthorizationPolicies.Administrator)]
    public async Task<IActionResult> SetStatus(
        int id,
        SetPPECategoryStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetPPECategoryStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }

}