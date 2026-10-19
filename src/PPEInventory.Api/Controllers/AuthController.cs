using MediatR;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Application.Features.Authentication.Commands.Login;
using Microsoft.AspNetCore.Authorization;
using PPEInventory.Application.Features.Authentication.Queries.GetCurrentUser;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(
            new GetCurrentUserQuery(),
            cancellationToken));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
}
