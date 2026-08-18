using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.InventoryCounts;

public static class InventoryCountMappings
{
    public static InventoryCountDto ToDto(
        this InventoryCount count)
    {
        var showComparison =
            count.Status != InventoryCountStatus.Draft;

        return new InventoryCountDto
        {
            Id = count.Id,
            Folio = count.Folio,

            WarehouseId =
                count.WarehouseId,

            WarehouseCode =
                count.Warehouse.Code,

            WarehouseName =
                count.Warehouse.Name,

            Status =
                count.Status,

            Notes =
                count.Notes,

            CreatedAt =
                count.CreatedAt,

            SubmittedAt =
                count.SubmittedAt,

            PostedAt =
                count.PostedAt,

            Items =
                count.Items
                    .OrderBy(x =>
                        x.PPEProduct.Name)
                    .Select(x =>
                        new InventoryCountItemDto
                        {
                            Id = x.Id,

                            PPEProductId =
                                x.PPEProductId,

                            Sku =
                                x.PPEProduct.Sku,

                            ProductName =
                                x.PPEProduct.Name,

                            CategoryName =
                                x.PPEProduct.Category.Name,

                            CountedQuantity =
                                x.CountedQuantity,

                            SystemQuantity =
                                showComparison
                                    ? x.SystemQuantitySnapshot
                                    : null,

                            Variance =
                                showComparison
                                    ? x.Variance
                                    : null,

                            CountedAt =
                                x.CountedAt
                        })
                    .ToArray()
        };
    }
}