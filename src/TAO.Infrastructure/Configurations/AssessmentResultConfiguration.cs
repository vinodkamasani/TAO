using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentResultConfiguration
    : IEntityTypeConfiguration<AssessmentResult>
{
    public void Configure(
        EntityTypeBuilder<AssessmentResult> builder)
    {
        builder.ToTable("AssessmentResults");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentSessionId)
            .IsRequired();

        builder.Property(x => x.OverallScore)
            .IsRequired();

        builder.Property(x => x.OverallConfidence)
            .IsRequired();

        builder.Property(x => x.Recommendation)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.ExecutiveSummary)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.GeneratedOn)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(x => x.ReviewedByUserId)
            .IsRequired(false);

        builder.Property(x => x.ReviewedOn)
            .HasColumnType("datetime2(7)")
            .IsRequired(false);

        builder.Property(x => x.RecruiterDecision)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.RecruiterComments)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.HasOne<AssessmentSession>()
            .WithOne()
            .HasForeignKey<AssessmentResult>(
                x => x.AssessmentSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AssessmentSessionId)
            .IsUnique();

        builder.HasIndex(x => x.ReviewedByUserId);
    }
}