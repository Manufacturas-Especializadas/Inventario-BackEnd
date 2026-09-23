using MediatR;

namespace PPEInventory.Application.Features.ProductSuppliers.Commands.ChangeStatus;

public record ChangeProductSupplierStatusCommand(
    int PPEProductId,
    int SupplierId,
    bool IsActive)
    : IRequest<ProductSupplierDto>;