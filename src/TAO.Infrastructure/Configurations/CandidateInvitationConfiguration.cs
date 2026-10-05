using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class CandidateInvitationConfiguration
    : IEntityTypeConfiguration<CandidateInvitation>
{
    public void Configure(
        EntityTypeBuilder<CandidateInvitation> builder)
    {
        builder.ToTable("CandidateInvitations");

        builder.ConfigurePrimaryKey();
        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .IsRequired();

        builder.Property(x => x.CandidateApplicationId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.SentOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.AcceptedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.ExpiresOn)
            .HasColumnType("datetime2(7)");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CandidateApplication>()
            .WithMany()
            .HasForeignKey(x => x.CandidateApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CandidateApplicationId)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.CampaignId,
            x.Status
        });

        builder.HasIndex(x => x.OrganizationId);
    }
}