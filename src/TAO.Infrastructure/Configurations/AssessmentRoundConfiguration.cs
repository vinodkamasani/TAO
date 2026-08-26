using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentRoundConfiguration
    : IEntityTypeConfiguration<AssessmentRound>
{
    public void Configure(
        EntityTypeBuilder<AssessmentRound> builder)
    {
        builder.ToTable("AssessmentRounds");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentStrategyId)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.Difficulty)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.DurationInMinutes)
            .IsRequired();

        builder.Property(x => x.TargetQuestionCount)
            .IsRequired();

        builder.Property(x => x.Competencies)
            .HasConversion(
                new ValueConverter<
                    IReadOnlyCollection<AssessmentRoundCompetency>,
                    string>(
                    value => JsonSerializer.Serialize(
                        value,
                        (JsonSerializerOptions?)null),

                    value => JsonSerializer.Deserialize<
                        List<AssessmentRoundCompetency>>(
                            value,
                            (JsonSerializerOptions?)null)
                        ?? new List<AssessmentRoundCompetency>()))
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.HasOne<AssessmentStrategy>()
            .WithMany()
            .HasForeignKey(x => x.AssessmentStrategyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.AssessmentStrategyId,
            x.Order
        })
        .IsUnique();
    }
}