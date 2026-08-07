using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class JobProfileConfiguration : IEntityTypeConfiguration<JobProfile>
{
    public void Configure(EntityTypeBuilder<JobProfile> builder)
    {
        builder.ToTable("JobProfiles");

        builder.ConfigurePrimaryKey();

        builder.ConfigureAuditColumns();
        builder.ConfigureAiGeneratedEntity();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .IsRequired();

        builder.Property(x => x.OriginalJobDescription)
            .IsRequired();


        builder.Property(x => x.GeneratedContent)
            .ConfigureMarkdownContent("GeneratedContent");

        builder.Property(x => x.StructuredProfile)
            .ConfigureStructuredContent("StructuredProfile");

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();


        builder.Property(x => x.ApprovedOn)
            .HasColumnType("datetime2(7)");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CampaignId)
            .IsUnique();
    }
}