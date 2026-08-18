using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class InventoryCountItemConfiguration
    : IEntityTypeConfiguration<InventoryCountItem>
{
    public void Configure(
        EntityTypeBuilder<InventoryCountItem> builder)
    {
        builder.ToTable(
            "InventoryCountItems",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_InventoryCountItems_CountedQuantity",
                    "[CountedQuantity] IS NULL OR [CountedQuantity] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryCountItems_SystemQuantity",
                    "[SystemQuantitySnapshot] IS NULL OR [SystemQuantitySnapshot] >= 0");
            });

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.InventoryCountId,
            x.PPEProductId
        })
        .IsUnique();

        builder.HasOne(x => x.InventoryCount)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InventoryCountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.InventoryCountItems)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CountedByUser)
            .WithMany()
            .HasForeignKey(x => x.CountedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}