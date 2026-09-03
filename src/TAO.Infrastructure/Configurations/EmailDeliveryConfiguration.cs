using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Campaigns;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class EmailDeliveryConfiguration
    : IEntityTypeConfiguration<EmailDelivery>
{
    public void Configure(EntityTypeBuilder<EmailDelivery> builder)
    {
        builder.ToTable("EmailDeliveries");

        builder.ConfigurePrimaryKey();
        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .IsRequired();

        builder.Property(x => x.CandidateApplicationId)
            .IsRequired();

        builder.Property(x => x.RecipientEmail)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Body)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.SentOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.FailedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.Campaign>()
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CandidateApplication>()
            .WithMany()
            .HasForeignKey(x => x.CandidateApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CampaignId,
            x.CandidateApplicationId,
            x.Status
        });
    }
}