using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class PurchaseOrderItemConfiguration
    : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(
        EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SupplierProductCode)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.PurchaseUnit)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.UnitsPerPackage)
            .IsRequired();

        builder.Property(x => x.OrderedPurchaseQuantity)
            .IsRequired();

        builder.Property(x => x.PurchaseUnitCost)
            .HasColumnType("decimal(18,4)");

        builder.HasIndex(x => new
        {
            x.PurchaseOrderId,
            x.PPEProductId
        })
        .IsUnique();

        builder.HasOne(x => x.PurchaseOrder)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.PurchaseOrderItems)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}