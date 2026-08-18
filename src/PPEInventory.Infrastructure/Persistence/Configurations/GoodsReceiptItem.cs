using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class GoodsReceiptItemConfiguration
    : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(
        EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.ToTable("GoodsReceiptItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReceivedQuantity)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.GoodsReceiptId,
            x.PPEProductId
        })
        .IsUnique();

        builder.HasIndex(x => x.PurchaseOrderItemId)
            .IsUnique();

        builder.HasOne(x => x.GoodsReceipt)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(x => x.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.GoodsReceiptItems)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}