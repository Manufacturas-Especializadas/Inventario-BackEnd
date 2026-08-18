using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class GoodsReceiptConfiguration
    : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(
        EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("GoodsReceipts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Folio)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasComputedColumnSql(
                "'GR-' + CONVERT(varchar(4), DATEPART(year, [ReceivedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)",
                stored: true);

        builder.HasIndex(x => x.Folio)
            .IsUnique();

        builder.HasIndex(x => x.PurchaseOrderId)
            .IsUnique();

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.PurchaseOrder)
            .WithOne(x => x.GoodsReceipt)
            .HasForeignKey<GoodsReceipt>(
                x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.GoodsReceipts)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReceivedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReceivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}