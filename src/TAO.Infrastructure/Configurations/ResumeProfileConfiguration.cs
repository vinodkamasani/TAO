using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class ResumeProfileConfiguration : IEntityTypeConfiguration<ResumeProfile>
{
    public void Configure(EntityTypeBuilder<ResumeProfile> builder)
    {
        builder.ToTable("ResumeProfiles");

        builder.ConfigurePrimaryKey();
        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.ApplicationId)
            .IsRequired();

        builder.Property(x => x.GeneratedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.StructuredContent)
            .ConfigureStructuredContent(nameof(ResumeProfile.StructuredContent));

        builder.HasOne<CandidateApplication>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrganizationId);

        builder.HasIndex(x => x.ApplicationId);

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.ApplicationId
        });
    }
}