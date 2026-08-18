using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class InventoryCountConfiguration
    : IEntityTypeConfiguration<InventoryCount>
{
    public void Configure(
        EntityTypeBuilder<InventoryCount> builder)
    {
        builder.ToTable("InventoryCounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Folio)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasComputedColumnSql(
                "'IC-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)",
                stored: true);

        builder.HasIndex(x => x.Folio)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Solo un conteo abierto por almacén.
        builder.HasIndex(x => x.WarehouseId)
            .HasFilter(
                "[Status] IN ('Draft', 'PendingReview')")
            .IsUnique();

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.InventoryCounts)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubmittedByUser)
            .WithMany()
            .HasForeignKey(x => x.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PostedByUser)
            .WithMany()
            .HasForeignKey(x => x.PostedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CancelledByUser)
            .WithMany()
            .HasForeignKey(x => x.CancelledByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}