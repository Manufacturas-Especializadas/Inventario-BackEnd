using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class OrganizationalUnitConfiguration
    : IEntityTypeConfiguration<OrganizationalUnit>
{
    public void Configure(
        EntityTypeBuilder<OrganizationalUnit> builder)
    {
        builder.ToTable(
            "OrganizationalUnits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.ParentId,
            x.Name
        })
        .IsUnique();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(
                DeleteBehavior.Restrict);
    }
}