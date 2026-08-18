using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class RoleConfiguration
    : IEntityTypeConfiguration<Role>
{
    public void Configure(
        EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Administrator",
                Description = "Full system administration.",
                IsActive = true
            },
            new Role
            {
                Id = 2,
                Name = "Production",
                Description = "Production operations.",
                IsActive = true
            },
            new Role
            {
                Id = 3,
                Name = "Warehouse",
                Description = "Warehouse operations.",
                IsActive = true
            },
            new Role
            {
                Id = 4,
                Name = "Viewer",
                Description = "Read-only access.",
                IsActive = true
            });
    }
}