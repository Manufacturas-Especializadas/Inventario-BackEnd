using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace PPEInventory.Application.Features.Departments.Commands.Create;

public record CreateDepartmentCommand(
    string Name,
    string? Description)
    : IRequest<DepartmentDto>;