using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class ProductSupplierConfiguration
    : IEntityTypeConfiguration<ProductSupplier>
{
    public void Configure(
        EntityTypeBuilder<ProductSupplier> builder)
    {
        builder.ToTable("ProductSuppliers");

        builder.HasKey(x => new
        {
            x.PPEProductId,
            x.SupplierId
        });

        builder.Property(x => x.SupplierProductCode)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.PackageBarcode)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.PurchaseUnit)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.UnitsPerPackage)
            .IsRequired();

        builder.Property(x => x.IsPreferred)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.ProductSuppliers)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.ProductSuppliers)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PPEProductId)
            .HasFilter("[IsPreferred] = 1 AND [IsActive] = 1")
            .IsUnique();
    }
}