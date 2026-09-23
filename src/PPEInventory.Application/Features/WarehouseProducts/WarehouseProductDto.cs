using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.WarehouseProducts;

public class WarehouseProductDto
{
    public int WarehouseId { get; set; }

    public string WarehouseCode { get; set; } = string.Empty;

    public string WarehouseName { get; set; } = string.Empty;

    public int PPEProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public static class WarehouseProductMappings
{
    public static WarehouseProductDto ToDto(
        this WarehouseProduct relation)
    {
        return new WarehouseProductDto
        {
            WarehouseId = relation.WarehouseId,
            WarehouseCode = relation.Warehouse.Code,
            WarehouseName = relation.Warehouse.Name,

            PPEProductId = relation.PPEProductId,
            Sku = relation.PPEProduct.Sku,
            ProductName = relation.PPEProduct.Name,

            IsActive = relation.IsActive,
            CreatedAt = relation.CreatedAt,
            UpdatedAt = relation.UpdatedAt
        };
    }
}