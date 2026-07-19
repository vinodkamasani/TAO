using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class ResumeScreeningConfiguration : IEntityTypeConfiguration<ResumeScreening>
{
    public void Configure(EntityTypeBuilder<ResumeScreening> builder)
    {
        builder.ToTable("ResumeScreenings");

        builder.ConfigurePrimaryKey();
        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.ApplicationId)
            .IsRequired();

        builder.Property(x => x.Content)
            .ConfigureMarkdownContent(nameof(ResumeScreening.Content));

        builder.Property(x => x.StructuredContent)
            .ConfigureStructuredContent(nameof(ResumeScreening.StructuredContent));

        builder.Property(x => x.GeneratedOn)
            .HasColumnType("datetime2(7)");

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