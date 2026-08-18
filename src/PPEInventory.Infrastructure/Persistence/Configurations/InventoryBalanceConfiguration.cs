using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class InventoryBalanceConfiguration
    : IEntityTypeConfiguration<InventoryBalance>
{
    public void Configure(
        EntityTypeBuilder<InventoryBalance> builder)
    {
        builder.ToTable(
            "InventoryBalances",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_InventoryBalances_OnHand",
                    "[OnHandQuantity] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryBalances_Reserved",
                    "[ReservedQuantity] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryBalances_Reserved_OnHand",
                    "[ReservedQuantity] <= [OnHandQuantity]");
            });

        builder.HasKey(x => new
        {
            x.WarehouseId,
            x.PPEProductId
        });

        builder.Property(x => x.OnHandQuantity)
            .IsRequired();

        builder.Property(x => x.ReservedQuantity)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.Ignore(x => x.AvailableQuantity);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.InventoryBalances)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.InventoryBalances)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}