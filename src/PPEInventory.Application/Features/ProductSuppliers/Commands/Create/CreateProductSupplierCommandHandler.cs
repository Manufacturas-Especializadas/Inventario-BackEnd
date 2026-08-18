using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.ProductSuppliers.Commands.Create;

public class CreateProductSupplierCommandHandler
    : IRequestHandler<
        CreateProductSupplierCommand,
        ProductSupplierDto>
{
    private readonly IPPEProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IProductSupplierRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateProductSupplierCommandHandler(
        IPPEProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IProductSupplierRepository repository,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _repository = repository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProductSupplierDto> Handle(
        CreateProductSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.GetByIdAsync(
                request.PPEProductId,
                cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"PPE product with id '{request.PPEProductId}' was not found.");
        }

        if (!product.IsActive)
        {
            throw new ConflictException(
                $"PPE product '{product.Sku}' is inactive.");
        }

        var supplier =
            await _supplierRepository.GetByIdAsync(
                request.SupplierId,
                cancellationToken);

        if (supplier is null)
        {
            throw new NotFoundException(
                $"Supplier with id '{request.SupplierId}' was not found.");
        }

        if (!supplier.IsActive)
        {
            throw new ConflictException(
                $"Supplier '{supplier.Name}' is inactive.");
        }

        if (await _repository.ExistsAsync(
            product.Id,
            supplier.Id,
            cancellationToken))
        {
            throw new ConflictException(
                $"Supplier '{supplier.Name}' is already assigned to product '{product.Sku}'.");
        }

        if (request.IsPreferred &&
            await _repository.HasPreferredSupplierAsync(
                product.Id,
                cancellationToken))
        {
            throw new ConflictException(
                $"Product '{product.Sku}' already has a preferred supplier.");
        }

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException(
                "Authenticated user was not found.");

        var relation = new ProductSupplier
        {
            PPEProductId = product.Id,
            SupplierId = supplier.Id,

            SupplierProductCode =
                Normalize(request.SupplierProductCode),

            PackageBarcode =
                Normalize(request.PackageBarcode),

            PurchaseUnit =
                request.PurchaseUnit.Trim(),

            UnitsPerPackage =
                request.UnitsPerPackage,

            IsPreferred =
                request.IsPreferred,

            IsActive = true,

            CreatedAt =
                _dateTimeProvider.UtcNow,

            CreatedByUserId =
                userId
        };

        await _repository.AddAsync(
            relation,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new ProductSupplierDto
        {
            PPEProductId = product.Id,
            Sku = product.Sku,
            ProductName = product.Name,

            SupplierId = supplier.Id,
            SupplierName = supplier.Name,

            SupplierProductCode =
                relation.SupplierProductCode,

            PackageBarcode =
                relation.PackageBarcode,

            PurchaseUnit =
                relation.PurchaseUnit,

            UnitsPerPackage =
                relation.UnitsPerPackage,

            IsPreferred =
                relation.IsPreferred,

            IsActive =
                relation.IsActive
        };
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}