using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentRoundEvaluationConfiguration
    : IEntityTypeConfiguration<AssessmentRoundEvaluation>
{
    public void Configure(
        EntityTypeBuilder<AssessmentRoundEvaluation> builder)
    {
        builder.ToTable("AssessmentRoundEvaluations");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentSessionRoundId)
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

        builder.Property(x => x.AssessmentSessionRoundId)
     .IsRequired();

        builder.HasOne<AssessmentSessionRound>()
            .WithOne()
            .HasForeignKey<AssessmentRoundEvaluation>(
                x => x.AssessmentSessionRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AssessmentSessionRoundId)
            .IsUnique();
    }
}