using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TAO.Domain.ValueObjects;

namespace TAO.Infrastructure.Persistence.Converters;

public sealed class StructuredContentConverter
    : ValueConverter<StructuredContent, string>
{
    public StructuredContentConverter()
        : base(
            structuredContent => structuredContent.Value,
            value => new StructuredContent(value))
    {
    }
}