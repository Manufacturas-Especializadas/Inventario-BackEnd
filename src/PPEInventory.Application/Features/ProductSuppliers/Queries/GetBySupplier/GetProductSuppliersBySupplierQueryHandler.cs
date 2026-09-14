using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features
    .ProductSuppliers.Queries.GetBySupplier;

public class GetProductSuppliersBySupplierQueryHandler
    : IRequestHandler<
        GetProductSuppliersBySupplierQuery,
        IReadOnlyList<ProductSupplierDto>>
{
    private readonly
        IProductSupplierRepository
        _repository;

    public GetProductSuppliersBySupplierQueryHandler(
        IProductSupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<ProductSupplierDto>>
        Handle(
            GetProductSuppliersBySupplierQuery request,
            CancellationToken cancellationToken)
    {
        var relations =
            await _repository
                .GetBySupplierIdAsync(
                    request.SupplierId,
                    cancellationToken);

        return relations
            .Select(x =>
                new ProductSupplierDto
                {
                    PPEProductId =
                        x.PPEProductId,

                    Sku =
                        x.PPEProduct.Sku,

                    ProductName =
                        x.PPEProduct.Name,

                    StockUnitId =
                        x.PPEProduct.StockUnitId,

                    StockUnit =
                        x.PPEProduct
                            .StockUnitOfMeasure
                            .Name,

                    StockUnitSymbol =
                        x.PPEProduct
                            .StockUnitOfMeasure
                            .Symbol,

                    SupplierId =
                        x.SupplierId,

                    SupplierName =
                        x.Supplier.Name,

                    SupplierProductCode =
                        x.SupplierProductCode,

                    PackageBarcode =
                        x.PackageBarcode,

                    PurchaseUnitId =
                        x.PurchaseUnitId,

                    PurchaseUnit =
                        x.PurchaseUnitOfMeasure
                            .Name,

                    PurchaseUnitSymbol =
                        x.PurchaseUnitOfMeasure
                            .Symbol,

                    UnitsPerPackage =
                        x.UnitsPerPackage,

                    IsPreferred =
                        x.IsPreferred,

                    IsActive =
                        x.IsActive
                })
            .ToList();
    }
}