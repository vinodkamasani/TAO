using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class AssessmentSessionConfiguration
    : IEntityTypeConfiguration<AssessmentSession>
{
    public void Configure(
        EntityTypeBuilder<AssessmentSession> builder)
    {
        builder.ToTable("AssessmentSessions");

        builder.ConfigurePrimaryKey();

        builder.Property(x => x.CandidateApplicationId)
            .IsRequired();

        builder.Property(x => x.AssessmentStrategyId)
            .IsRequired();

        builder.Property(x => x.StrategySnapshot)
                .HasConversion(
                    snapshot => snapshot.Value,
                    value => AssessmentStrategySnapshot.Create(value))
                .HasColumnType("nvarchar(max)")
                .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.CurrentSessionRoundId)
            .IsRequired(false);

        builder.Property(x => x.CurrentQuestionId)
            .IsRequired(false);

        builder.Property(x => x.ConsentAcceptedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.ConsentVersion)
            .IsRequired();

        builder.Property(x => x.StartedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.CompletedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.AssessmentExpiresOn)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(x => x.LastActivityOn)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(x => x.HasUsedInterruptionWindow)
            .IsRequired();

        builder.Property(x => x.IsInterrupted)
            .IsRequired();

        builder.HasOne<CandidateApplication>()
            .WithMany()
            .HasForeignKey(x => x.CandidateApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AssessmentStrategy>()
            .WithMany()
            .HasForeignKey(x => x.AssessmentStrategyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AssessmentSessionRound>()
            .WithMany()
            .HasForeignKey(x => x.CurrentSessionRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AssessmentQuestion>()
            .WithMany()
            .HasForeignKey(x => x.CurrentQuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CandidateApplicationId);

        builder.HasIndex(x => x.AssessmentStrategyId);

        builder.HasIndex(x => new
        {
            x.Status,
            x.AssessmentExpiresOn
        });
    }
}