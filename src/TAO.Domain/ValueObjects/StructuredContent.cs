using TAO.Domain.Common;

namespace TAO.Domain.ValueObjects;

public sealed record StructuredContent
{
    public StructuredContent(string value)
    {
        Value = Guard.AgainstNullOrWhiteSpace(value, nameof(Value));
    }

    public string Value { get; }

    public override string ToString() => Value;

}


