using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class PPERequestConfiguration
    : IEntityTypeConfiguration<PPERequest>
{
    public void Configure(
        EntityTypeBuilder<PPERequest> builder)
    {
        builder.ToTable("PPERequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Folio)
            .HasMaxLength(25)
            .IsUnicode(false)
            .HasComputedColumnSql(
                "'EPP-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)",
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

        // These workflow timestamps are generated in UTC. SQL Server datetime2
        // preserves their value but not DateTime.Kind; restore it only on reads.
        // EF preserves nulls and reuses this converter for nullable properties.
        var utcDateTimeConverter = new ValueConverter<DateTime, DateTime>(
            value => value,
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

        builder.Property(x => x.CreatedAt)
            .HasConversion(utcDateTimeConverter)
            .IsRequired();

        builder.Property(x => x.ApprovedAt)
            .HasConversion(utcDateTimeConverter);

        builder.Property(x => x.RejectedAt)
            .HasConversion(utcDateTimeConverter);

        builder.Property(x => x.DeliveredAt)
            .HasConversion(utcDateTimeConverter);

        builder.Property(x => x.CancelledAt)
            .HasConversion(utcDateTimeConverter);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RequestReason)
            .WithMany(x => x.PPERequests)
            .HasForeignKey(x => x.RequestReasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeliveredByUser)
            .WithMany()
            .HasForeignKey(x => x.DeliveredByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedByUser)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RejectedByUser)
            .WithMany()
            .HasForeignKey(x => x.RejectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CancelledByUser)
            .WithMany()
            .HasForeignKey(x => x.CancelledByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RequestedForOrganizationalUnit)
    .WithMany()
    .HasForeignKey(x => x.RequestedForOrganizationalUnitId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}
