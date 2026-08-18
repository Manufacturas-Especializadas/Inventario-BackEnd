using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class InventoryAdjustmentItemConfiguration
    : IEntityTypeConfiguration<InventoryAdjustmentItem>
{
    public void Configure(
        EntityTypeBuilder<InventoryAdjustmentItem> builder)
    {
        builder.ToTable(
            "InventoryAdjustmentItems",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_InventoryAdjustmentItems_QuantityAdjustment",
                    "[QuantityAdjustment] <> 0");

                table.HasCheckConstraint(
                    "CK_InventoryAdjustmentItems_PreviousOnHand",
                    "[PreviousOnHandQuantity] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryAdjustmentItems_NewOnHand",
                    "[NewOnHandQuantity] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryAdjustmentItems_Reserved",
                    "[ReservedQuantitySnapshot] >= 0");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityAdjustment)
            .IsRequired();

        builder.Property(x => x.PreviousOnHandQuantity)
            .IsRequired();

        builder.Property(x => x.NewOnHandQuantity)
            .IsRequired();

        builder.Property(x => x.ReservedQuantitySnapshot)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.InventoryAdjustmentId,
            x.PPEProductId
        })
        .IsUnique();

        builder.HasOne(x => x.InventoryAdjustment)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InventoryAdjustmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.InventoryAdjustmentItems)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}