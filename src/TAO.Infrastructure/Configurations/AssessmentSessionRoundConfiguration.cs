using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentSessionRoundConfiguration
    : IEntityTypeConfiguration<AssessmentSessionRound>
{
    public void Configure(
        EntityTypeBuilder<AssessmentSessionRound> builder)
    {
        builder.ToTable("AssessmentSessionRounds");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentSessionId)
            .IsRequired();

        builder.Property(x => x.AssessmentRoundId)
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

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.StartedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.ExpiresOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.CompletedOn)
            .HasColumnType("datetime2(7)");

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

        builder.HasOne<AssessmentSession>()
            .WithMany()
            .HasForeignKey(x => x.AssessmentSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AssessmentRound>()
            .WithMany()
            .HasForeignKey(x => x.AssessmentRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.AssessmentSessionId,
            x.Order
        })
        .IsUnique();

        builder.HasIndex(x => x.AssessmentRoundId);
    }
}