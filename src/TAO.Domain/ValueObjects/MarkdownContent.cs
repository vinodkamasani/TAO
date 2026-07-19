using TAO.Domain.Common;

namespace TAO.Domain.ValueObjects;

public sealed record MarkdownContent
{
    public MarkdownContent(string value)
    {
        Value = Guard.AgainstNullOrWhiteSpace(value, nameof(Value));
    }

    public string Value { get; }

    public override string ToString() => Value;

    public static implicit operator string(MarkdownContent content)
        => content.Value;

    public static implicit operator MarkdownContent(string value)
        => new(value);
}