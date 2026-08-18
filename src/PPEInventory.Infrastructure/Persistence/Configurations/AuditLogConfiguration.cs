using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(
        EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.OldValuesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValuesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => new
        {
            x.EntityName,
            x.EntityId
        });

        builder.HasIndex(x => new
        {
            x.PerformedByUserId,
            x.CreatedAt
        });

        builder.HasOne(x => x.PerformedByUser)
            .WithMany()
            .HasForeignKey(x => x.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}