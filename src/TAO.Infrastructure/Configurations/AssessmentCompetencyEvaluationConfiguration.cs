using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentCompetencyEvaluationConfiguration
    : IEntityTypeConfiguration<AssessmentCompetencyEvaluation>
{
    public void Configure(
        EntityTypeBuilder<AssessmentCompetencyEvaluation> builder)
    {
        builder.ToTable("AssessmentCompetencyEvaluations");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentResultId)
            .IsRequired();

        builder.Property(x => x.CompetencyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Score)
            .IsRequired();

        builder.Property(x => x.MinimumPassPercentage)
            .IsRequired();

        builder.HasOne<AssessmentResult>()
            .WithMany()
            .HasForeignKey(x => x.AssessmentResultId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.AssessmentResultId,
            x.CompetencyName
        })
        .IsUnique();
    }
}