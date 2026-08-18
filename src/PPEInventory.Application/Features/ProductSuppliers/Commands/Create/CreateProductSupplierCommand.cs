using MediatR;

namespace PPEInventory.Application.Features.ProductSuppliers.Commands.Create;

public record CreateProductSupplierCommand(
    int PPEProductId,
    int SupplierId,
    string? SupplierProductCode,
    string? PackageBarcode,
    string PurchaseUnit,
    int UnitsPerPackage,
    bool IsPreferred)
    : IRequest<ProductSupplierDto>;