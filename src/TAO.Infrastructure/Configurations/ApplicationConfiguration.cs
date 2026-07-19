using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable("Application");

        builder.ConfigurePrimaryKey();

        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .IsRequired();

        builder.Property(x => x.CandidateName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(x => x.CurrentCompany)
            .HasMaxLength(200);

        builder.Property(x => x.CurrentLocation)
            .HasMaxLength(200);

        builder.Property(x => x.OverallMatchPercentage)
            .IsRequired();

        builder.Property(x => x.IsRecommended)
            .IsRequired();

        builder.Property(x => x.ResumeUploadedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.LastScreenedOn)
            .HasColumnType("datetime2(7)");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CampaignId);
    }
}