using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Users.Commands.Create;
using PPEInventory.Application.Features.Users.Queries.GetAll;
using PPEInventory.Application.Features.Users.Queries.GetById;
using PPEInventory.Application.Features.Users.Commands.SetStatus;
using PPEInventory.Application.Features.Users.Commands.Update;
using PPEInventory.Application.Features.Users.Commands.ResetPassword;
using PPEInventory.Application.Features.Users.Commands.SetRoles;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(
    Policy = AuthorizationPolicies.Administrator)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;


    public UsersController(
        IMediator mediator)
    {
        _mediator =
            mediator;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetUsersQuery(),
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetUserByIdQuery(id),
                cancellationToken);

        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId =
            await _mediator.Send(
                command,
                cancellationToken);

        return Ok(
            new
            {
                userId
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
    int id,
    UpdateUserRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new UpdateUserCommand(
                    id,
                    request.Username),
                cancellationToken);

        return Ok(result);
    }


    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> SetStatus(
        int id,
        SetUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetUserStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/roles")]
    public async Task<IActionResult> SetRoles(
    int id,
    SetUserRolesRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetUserRolesCommand(
                    id,
                    request.Roles),
                cancellationToken);

        return Ok(result);
    }


    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ResetPassword(
        int id,
        ResetUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new ResetUserPasswordCommand(
                    id,
                    request.NewPassword),
                cancellationToken);

        return Ok(result);
    }

}