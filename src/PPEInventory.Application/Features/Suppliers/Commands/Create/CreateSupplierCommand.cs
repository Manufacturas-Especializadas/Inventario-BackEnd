using MediatR;

namespace PPEInventory.Application.Features.Suppliers.Commands.Create;

public record CreateSupplierCommand(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone)
    : IRequest<SupplierDto>;