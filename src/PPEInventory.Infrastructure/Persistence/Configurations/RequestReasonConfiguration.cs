using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence.Configurations;

public class RequestReasonConfiguration
    : IEntityTypeConfiguration<RequestReason>
{
    public void Configure(
        EntityTypeBuilder<RequestReason> builder)
    {
        builder.ToTable("RequestReasons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasData(
            new RequestReason
            {
                Id = 1,
                Code = RequestReasonCodes.InitialAssignment,
                Name = "Initial Assignment",
                Description = "Initial PPE assignment.",
                IsActive = true
            },
            new RequestReason
            {
                Id = 2,
                Code = RequestReasonCodes.ScheduledReplacement,
                Name = "Scheduled Replacement",
                Description = "Replacement according to scheduled useful life.",
                IsActive = true
            },
            new RequestReason
            {
                Id = 3,
                Code = RequestReasonCodes.Wear,
                Name = "Wear",
                Description = "Replacement due to normal wear.",
                IsActive = true
            },
            new RequestReason
            {
                Id = 4,
                Code = RequestReasonCodes.Damage,
                Name = "Damage",
                Description = "Replacement due to damage.",
                IsActive = true
            },
            new RequestReason
            {
                Id = 5,
                Code = RequestReasonCodes.Lost,
                Name = "Lost",
                Description = "Replacement because PPE was lost.",
                IsActive = true
            },
            new RequestReason
            {
                Id = 6,
                Code = RequestReasonCodes.JobChange,
                Name = "Job Change",
                Description = "PPE required because of job or position change.",
                IsActive = true
            },
            new RequestReason
            {
                Id = 7,
                Code = RequestReasonCodes.Other,
                Name = "Other",
                Description = "Other justified reason.",
                IsActive = true
            });
    }
}