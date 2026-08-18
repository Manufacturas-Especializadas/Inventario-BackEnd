using MediatR;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Application.Features.Authentication.Commands.Login;
using Microsoft.AspNetCore.Authorization;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            isAuthenticated =
                _currentUserService.IsAuthenticated,

            userId =
                _currentUserService.UserId,

            employeeId =
                _currentUserService.EmployeeId,

            employeeNumber =
                _currentUserService.EmployeeNumber,

            name =
                _currentUserService.Name,

            username =
                _currentUserService.Username,

            roles =
                _currentUserService.Roles
        });
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