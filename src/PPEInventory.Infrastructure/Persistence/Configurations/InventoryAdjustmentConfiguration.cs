using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class InventoryAdjustmentConfiguration
    : IEntityTypeConfiguration<InventoryAdjustment>
{
    public void Configure(
        EntityTypeBuilder<InventoryAdjustment> builder)
    {
        builder.ToTable("InventoryAdjustments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Folio)
            .HasMaxLength(25)
            .IsUnicode(false)
            .HasComputedColumnSql(
                "'ADJ-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)",
                stored: true);

        builder.HasIndex(x => x.Folio)
            .IsUnique();

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.InventoryAdjustments)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}