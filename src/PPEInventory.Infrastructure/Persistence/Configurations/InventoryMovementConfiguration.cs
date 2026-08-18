using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class InventoryMovementConfiguration
    : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(
        EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MovementType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(x => x.ReferenceType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitCost)
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.WarehouseId,
            x.PPEProductId,
            x.CreatedAt
        });

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.InventoryMovements)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.InventoryMovements)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}