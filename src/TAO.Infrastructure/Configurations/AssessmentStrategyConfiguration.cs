using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Configurations;


public sealed class AssessmentStrategyConfiguration
    : IEntityTypeConfiguration<AssessmentStrategy>
{
    public void Configure(EntityTypeBuilder<AssessmentStrategy> builder)
    {
        builder.ToTable("AssessmentStrategies");

        builder.ConfigurePrimaryKey();

        builder.ConfigureAuditColumns();
        builder.ConfigureAiGeneratedEntity();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .IsRequired();

        builder.Property(x => x.AssessmentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Content)
            .ConfigureMarkdownContent("GeneratedContent");

        builder.Property(x => x.StructuredContent)
            .ConfigureStructuredContent("StructuredData");

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

        builder.HasIndex(x => x.OrganizationId);

        builder.HasIndex(x => x.CampaignId)
            .IsUnique();
    }
}