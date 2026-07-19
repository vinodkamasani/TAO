using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TAO.Domain.ValueObjects;

namespace TAO.Infrastructure.Persistence.Converters;

public sealed class MarkdownContentConverter
    : ValueConverter<MarkdownContent, string>
{
    public MarkdownContentConverter()
        : base(
            markdown => markdown.Value,
            value => new MarkdownContent(value))
    {
    }
}