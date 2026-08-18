using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration
    : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(
        EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Folio)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasComputedColumnSql(
                "'PO-' + CONVERT(varchar(4), DATEPART(year, [OrderDate])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)",
                stored: true);

        builder.HasIndex(x => x.Folio)
            .IsUnique();

        builder.Property(x => x.PurchaseOrderNumber)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.OrderDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.ConfirmedDeliveryDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.SupplierConfirmedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3)
            .IsUnicode(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        builder.HasIndex(x => new
        {
            x.SupplierId,
            x.PurchaseOrderNumber
        })
        .IsUnique();

        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.PurchaseOrders)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CancelledByUser)
            .WithMany()
            .HasForeignKey(x => x.CancelledByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}