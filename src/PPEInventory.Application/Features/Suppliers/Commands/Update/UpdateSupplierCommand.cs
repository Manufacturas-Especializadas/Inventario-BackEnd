using MediatR;

namespace PPEInventory.Application.Features.Suppliers.Commands.Update;

public record UpdateSupplierCommand(
    int Id,
    string Name,
    string? ContactName,
    string? Email,
    string? Phone)
    : IRequest<SupplierDto>;