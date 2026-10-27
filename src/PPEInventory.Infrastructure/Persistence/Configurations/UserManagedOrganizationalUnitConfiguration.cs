using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class UserManagedOrganizationalUnitConfiguration
    : IEntityTypeConfiguration<UserManagedOrganizationalUnit>
{
    public void Configure(EntityTypeBuilder<UserManagedOrganizationalUnit> builder)
    {
        builder.ToTable("UserManagedOrganizationalUnits");
        builder.HasKey(x => new { x.UserId, x.OrganizationalUnitId });
        builder.HasOne(x => x.User)
            .WithMany(x => x.ManagedOrganizationalUnits)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.OrganizationalUnit)
            .WithMany()
            .HasForeignKey(x => x.OrganizationalUnitId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.OrganizationalUnitId);
    }
}
