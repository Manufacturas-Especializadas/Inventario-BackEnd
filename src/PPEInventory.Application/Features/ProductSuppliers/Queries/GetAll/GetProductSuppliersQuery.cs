using MediatR;
using PPEInventory.Application.Features.ProductSuppliers;

public record GetProductSuppliersQuery(int PPEProductId): IRequest<IReadOnlyList<ProductSupplierDto>>;