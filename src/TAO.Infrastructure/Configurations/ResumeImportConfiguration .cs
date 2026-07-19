using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class ResumeImportConfiguration : IEntityTypeConfiguration<ResumeImport>
{
    public void Configure(EntityTypeBuilder<ResumeImport> builder)
    {
        builder.ToTable("ResumeImport");

        builder.ConfigurePrimaryKey();

        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .IsRequired();

        builder.Property(x => x.TotalFiles)
            .IsRequired();

        builder.Property(x => x.SuccessfulFiles)
            .IsRequired();

        builder.Property(x => x.FailedFiles)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();

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