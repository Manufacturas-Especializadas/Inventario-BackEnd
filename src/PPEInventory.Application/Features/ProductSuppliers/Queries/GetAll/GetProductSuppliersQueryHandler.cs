using MediatR;
using PPEInventory.Application.Features.Suppliers;
using PPEInventory.Application.Features.Suppliers.Queries.GetAll;
using PPEInventory.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPEInventory.Application.Features.ProductSuppliers.Queries.GetAll
{
    public class GetProductSuppliersQueryHandler
    : IRequestHandler<GetProductSuppliersQuery,
        IReadOnlyList<ProductSupplierDto>>
    {
        private readonly IProductSupplierRepository _repository;

        public GetProductSuppliersQueryHandler(
            IProductSupplierRepository repository)
        {
            _repository = repository;
        }


        public async Task<IReadOnlyList<ProductSupplierDto>> Handle(
        GetProductSuppliersQuery request,
        CancellationToken cancellationToken)
        {
            var relations = await _repository.GetByProductIdAsync(
                request.PPEProductId,
                cancellationToken);

            return relations
    .Select(x => new ProductSupplierDto
    {
        PPEProductId = x.PPEProductId,
        Sku = x.PPEProduct.Sku,
        ProductName = x.PPEProduct.Name,
        StockUnitId = x.PPEProduct.StockUnitId,

        StockUnit = x.PPEProduct
            .StockUnitOfMeasure
            .Name,

        StockUnitSymbol = x.PPEProduct
            .StockUnitOfMeasure
            .Symbol,

        SupplierId = x.SupplierId,
        SupplierName = x.Supplier.Name,

        SupplierProductCode =
            x.SupplierProductCode,

        PackageBarcode =
            x.PackageBarcode,

        PurchaseUnitId =
            x.PurchaseUnitId,

        PurchaseUnit =
            x.PurchaseUnitOfMeasure.Name,

        PurchaseUnitSymbol =
            x.PurchaseUnitOfMeasure.Symbol,

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



}
