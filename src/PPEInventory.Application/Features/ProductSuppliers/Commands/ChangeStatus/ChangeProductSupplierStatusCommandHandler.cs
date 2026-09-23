using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.ProductSuppliers.Commands.ChangeStatus;

public class ChangeProductSupplierStatusCommandHandler
    : IRequestHandler<
        ChangeProductSupplierStatusCommand,
        ProductSupplierDto>
{
    private readonly IProductSupplierRepository _repository;

    public ChangeProductSupplierStatusCommandHandler(
        IProductSupplierRepository repository)
    {
        _repository = repository;
    }


    public async Task<ProductSupplierDto> Handle(
        ChangeProductSupplierStatusCommand request,
        CancellationToken cancellationToken)
    {
        var relation =
            await _repository.GetAsync(
                request.PPEProductId,
                request.SupplierId,
                cancellationToken);

        if (relation is null)
        {
            throw new NotFoundException(
                "The supplier is not assigned to the product.");
        }


        if (relation.IsActive == request.IsActive)
        {
            return ToDto(relation);
        }


        if (request.IsActive)
        {
            if (!relation.PPEProduct.IsActive)
            {
                throw new ConflictException(
                    $"PPE product '{relation.PPEProduct.Sku}' is inactive.");
            }

            if (!relation.Supplier.IsActive)
            {
                throw new ConflictException(
                    $"Supplier '{relation.Supplier.Name}' is inactive.");
            }


            if (relation.IsPreferred &&
                await _repository.HasOtherPreferredSupplierAsync(
                    relation.PPEProductId,
                    relation.SupplierId,
                    cancellationToken))
            {
                throw new ConflictException(
                    $"Product '{relation.PPEProduct.Sku}' already has another active preferred supplier.");
            }
        }


        relation.IsActive =
            request.IsActive;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return ToDto(relation);
    }


    private static ProductSupplierDto ToDto(
        Domain.Entities.ProductSupplier relation)
    {
        return new ProductSupplierDto
        {
            PPEProductId =
                relation.PPEProductId,

            Sku =
                relation.PPEProduct.Sku,

            ProductName =
                relation.PPEProduct.Name,

            StockUnitId =
                relation.PPEProduct.StockUnitId,

            StockUnit =
                relation.PPEProduct
                    .StockUnitOfMeasure
                    .Name,

            StockUnitSymbol =
                relation.PPEProduct
                    .StockUnitOfMeasure
                    .Symbol,

            SupplierId =
                relation.SupplierId,

            SupplierName =
                relation.Supplier.Name,

            SupplierProductCode =
                relation.SupplierProductCode,

            PackageBarcode =
                relation.PackageBarcode,

            PurchaseUnitId =
                relation.PurchaseUnitId,

            PurchaseUnit =
                relation.PurchaseUnitOfMeasure
                    .Name,

            PurchaseUnitSymbol =
                relation.PurchaseUnitOfMeasure
                    .Symbol,

            UnitsPerPackage =
                relation.UnitsPerPackage,

            IsPreferred =
                relation.IsPreferred,

            IsActive =
                relation.IsActive
        };
    }
}