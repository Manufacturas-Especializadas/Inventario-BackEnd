using MediatR;

namespace PPEInventory.Application.Features
    .ProductSuppliers.Queries.GetBySupplier;

public record GetProductSuppliersBySupplierQuery(
    int SupplierId)
    : IRequest<
        IReadOnlyList<ProductSupplierDto>>;