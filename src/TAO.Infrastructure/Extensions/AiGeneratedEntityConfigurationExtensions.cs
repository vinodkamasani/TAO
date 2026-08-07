using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Common;

namespace TAO.Infrastructure.Persistence.Extensions;

public static class AiGeneratedEntityConfigurationExtensions
{
    public static EntityTypeBuilder<T> ConfigureAiGeneratedEntity<T>(
        this EntityTypeBuilder<T> builder)
        where T : AiGeneratedArtifact
    {
        builder.Property(x => x.Prompt)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.RawResponse)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.ProviderName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ModelName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PromptVersion)
            .IsRequired();

        builder.Property(x => x.GeneratedOn)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        return builder;
    }
}