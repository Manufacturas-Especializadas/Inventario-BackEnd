using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class WarehouseProductConfiguration
    : IEntityTypeConfiguration<WarehouseProduct>
{
    public void Configure(
        EntityTypeBuilder<WarehouseProduct> builder)
    {
        builder.ToTable("WarehouseProducts");

        builder.HasKey(x => new
        {
            x.WarehouseId,
            x.PPEProductId
        });


        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();


        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.WarehouseProducts)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.PPEProduct)
            .WithMany(x => x.WarehouseProducts)
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(x => x.PPEProductId);
    }
}