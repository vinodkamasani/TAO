using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentQuestionEvaluationConfiguration
    : IEntityTypeConfiguration<AssessmentQuestionEvaluation>
{
    public void Configure(
        EntityTypeBuilder<AssessmentQuestionEvaluation> builder)
    {
        builder.ToTable("AssessmentQuestionEvaluations");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentQuestionId)
            .IsRequired();

        builder.Property(x => x.Score)
            .IsRequired();

        builder.Property(x => x.Confidence)
            .IsRequired();

        builder.Property(x => x.Strengths)
            .HasConversion(
                new ValueConverter<
                    IReadOnlyCollection<string>,
                    string>(
                    value => JsonSerializer.Serialize(
                        value,
                        (JsonSerializerOptions?)null),

                    value => JsonSerializer.Deserialize<
                        List<string>>(
                            value,
                            (JsonSerializerOptions?)null)
                        ?? new List<string>()))
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Gaps)
            .HasConversion(
                new ValueConverter<
                    IReadOnlyCollection<string>,
                    string>(
                    value => JsonSerializer.Serialize(
                        value,
                        (JsonSerializerOptions?)null),

                    value => JsonSerializer.Deserialize<
                        List<string>>(
                            value,
                            (JsonSerializerOptions?)null)
                        ?? new List<string>()))
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Evidence)
            .HasConversion(
                new ValueConverter<
                    IReadOnlyCollection<string>,
                    string>(
                    value => JsonSerializer.Serialize(
                        value,
                        (JsonSerializerOptions?)null),

                    value => JsonSerializer.Deserialize<
                        List<string>>(
                            value,
                            (JsonSerializerOptions?)null)
                        ?? new List<string>()))
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Competencies)
            .HasConversion(
                new ValueConverter<
                    IReadOnlyCollection<AssessmentQuestionCompetencyEvaluation>,
                    string>(
                    value => JsonSerializer.Serialize(
                        value,
                        (JsonSerializerOptions?)null),

                    value => JsonSerializer.Deserialize<
                        List<AssessmentQuestionCompetencyEvaluation>>(
                            value,
                            (JsonSerializerOptions?)null)
                        ?? new List<AssessmentQuestionCompetencyEvaluation>()))
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        // Audit fields inherited from Entity.
        builder.Property(x => x.CreatedBy)
            .IsRequired(false);

        builder.Property(x => x.CreatedOn)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(x => x.ModifiedBy)
            .IsRequired(false);

        builder.Property(x => x.ModifiedOn)
            .HasColumnType("datetime2(7)")
            .IsRequired(false);

        builder.HasOne<AssessmentQuestion>()
            .WithOne()
            .HasForeignKey<AssessmentQuestionEvaluation>(
                x => x.AssessmentQuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AssessmentQuestionId)
            .IsUnique();
    }
}