using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Common;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure.Persistence.Converters;

namespace TAO.Infrastructure.Persistence.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static void ConfigurePrimaryKey<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
    }

    public static void ConfigureAuditColumns<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.Property(x => x.CreatedOn)
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.ModifiedOn)
            .HasColumnType("datetime2(7)");
    }

    public static PropertyBuilder<MarkdownContent> ConfigureMarkdownContent(
    this PropertyBuilder<MarkdownContent> propertyBuilder,
    string columnName)
    {
        return propertyBuilder
            .HasConversion<MarkdownContentConverter>()
            .HasColumnName(columnName)
            .IsRequired();
    }

    public static PropertyBuilder<StructuredContent> ConfigureStructuredContent(
        this PropertyBuilder<StructuredContent> propertyBuilder,
        string columnName)
    {
        return propertyBuilder
            .HasConversion<StructuredContentConverter>()
            .HasColumnName(columnName)
            .IsRequired();
    }
}