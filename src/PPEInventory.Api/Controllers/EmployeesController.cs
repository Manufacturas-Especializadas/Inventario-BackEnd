using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Api.Authorization;
using PPEInventory.Application.Features.Employees.Commands.Create;
using PPEInventory.Application.Features.Employees.Queries.GetAll;
using PPEInventory.Application.Features.Employees.Queries.GetByEmployeeNumber;
using PPEInventory.Application.Features.Employees.Commands.Update;
using PPEInventory.Application.Features.Employees.Commands.SetStatus;

namespace PPEInventory.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetEmployeesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("by-number/{employeeNumber}")]
    [Authorize(Policy = AuthorizationPolicies.Viewer)]
    public async Task<IActionResult> GetByEmployeeNumber(
        string employeeNumber,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetEmployeeByNumberQuery(employeeNumber),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Production)]
    public async Task<IActionResult> Create(
        CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.Production)]
    public async Task<IActionResult> Update(
    int id,
    UpdateEmployeeCommand command,
    CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(
                new
                {
                    message = "Route employee id does not match request employee id."
                });
        }

        var result =
            await _mediator.Send(
                command,
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(
    Policy = AuthorizationPolicies.Production)]
    public async Task<IActionResult> SetStatus(
    int id,
    SetEmployeeStatusRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new SetEmployeeStatusCommand(
                    id,
                    request.IsActive),
                cancellationToken);

        return Ok(result);
    }

}