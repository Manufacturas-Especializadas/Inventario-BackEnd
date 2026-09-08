using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class OrganizationalUnitPPELimitConfiguration
    : IEntityTypeConfiguration<
        OrganizationalUnitPPELimit>
{
    public void Configure(
        EntityTypeBuilder<
            OrganizationalUnitPPELimit> builder)
    {
        builder.ToTable(
            "OrganizationalUnitPPELimits");

        builder.HasKey(x => x.Id);

        builder.Property(
         x => x.MaxQuantityPerCycle)
     .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.OrganizationalUnitId,
            x.PPEProductId
        })
        .IsUnique();

        builder.HasOne(
                x => x.OrganizationalUnit)
            .WithMany(x => x.PPELimits)
            .HasForeignKey(
                x => x.OrganizationalUnitId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasOne(x => x.PPEProduct)
            .WithMany(
                x => x.OrganizationalUnitLimits)
            .HasForeignKey(
                x => x.PPEProductId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.ToTable(
    tableBuilder =>
    {
        tableBuilder.HasCheckConstraint(
            "CK_OrganizationalUnitPPELimits_MaxQuantity",
            "[MaxQuantityPerCycle] > 0");
    });
    }
}