using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentQuestionConfiguration
    : IEntityTypeConfiguration<AssessmentQuestion>
{
    public void Configure(
        EntityTypeBuilder<AssessmentQuestion> builder)
    {
        builder.ToTable("AssessmentQuestions");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.AssessmentSessionRoundId)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.PrimaryQuestion)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.CandidateCode)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.StartedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.CompletedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.Competencies)
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

        builder.Property(x => x.Conversation)
            .HasConversion(
                new ValueConverter<
                    ConversationContent?,
                    string?>(
                    value => value == null
                        ? null
                        : value.Value,

                    value => value == null
                        ? null
                        : ConversationContent.Create(value)))
            .HasColumnType("nvarchar(max)");

        builder.HasOne<AssessmentSessionRound>()
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.AssessmentSessionRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.AssessmentSessionRoundId,
            x.Order
        })
        .IsUnique();
    }
}