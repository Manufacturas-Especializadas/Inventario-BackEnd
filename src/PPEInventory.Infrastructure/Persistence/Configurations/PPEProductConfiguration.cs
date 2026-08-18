using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class PPEProductConfiguration
    : IEntityTypeConfiguration<PPEProduct>
{
    public void Configure(
        EntityTypeBuilder<PPEProduct> builder)
    {
        builder.ToTable("PPEProducts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku)
            .HasColumnName("SKU")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasComputedColumnSql(
                "'EPP-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)",
                stored: true);

        builder.HasIndex(x => x.Sku)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Size)
            .HasMaxLength(50);

        builder.Property(x => x.Color)
            .HasMaxLength(50);

        builder.Property(x => x.Model)
            .HasMaxLength(100);

        builder.Property(x => x.Specification)
            .HasMaxLength(250);

        builder.Property(x => x.StockUnit)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.MinimumStock)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}