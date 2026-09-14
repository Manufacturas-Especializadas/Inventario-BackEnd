using MediatR;

namespace PPEInventory.Application.Features.Suppliers.Commands.SetStatus;

public record SetSupplierStatusCommand(
    int Id,
    bool IsActive)
    : IRequest<SupplierDto>;