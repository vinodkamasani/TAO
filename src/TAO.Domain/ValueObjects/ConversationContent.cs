namespace TAO.Domain.ValueObjects;

public sealed record ConversationContent
{
    private ConversationContent(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ConversationContent Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Conversation content cannot be empty.",
                nameof(value));
        }

        return new ConversationContent(value);
    }

    public override string ToString()
    {
        return Value;
    }
}