using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class PPERequestItemConfiguration
    : IEntityTypeConfiguration<PPERequestItem>
{
    public void Configure(
        EntityTypeBuilder<PPERequestItem> builder)
    {
        builder.ToTable(
            "PPERequestItems",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_PPERequestItems_Quantity",
                    "[Quantity] > 0");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.PPERequestId,
            x.PPEProductId
        })
        .IsUnique();

        builder.HasOne(x => x.PPERequest)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PPERequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PPEProduct)
            .WithMany()
            .HasForeignKey(x => x.PPEProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}