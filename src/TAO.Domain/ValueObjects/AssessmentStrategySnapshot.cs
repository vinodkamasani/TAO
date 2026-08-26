namespace TAO.Domain.ValueObjects;

public sealed record AssessmentStrategySnapshot
{
    private AssessmentStrategySnapshot(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static AssessmentStrategySnapshot Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Assessment strategy snapshot cannot be empty.",
                nameof(value));
        }

        return new AssessmentStrategySnapshot(value);
    }

    public override string ToString()
    {
        return Value;
    }
}