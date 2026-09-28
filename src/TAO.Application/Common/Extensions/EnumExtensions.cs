public static class EnumExtensions
{
    public static bool TryParseNormalized<TEnum>(
        string? value,
        out TEnum result)
        where TEnum : struct, Enum
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalizedValue = Normalize(value);

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            if (Normalize(enumValue.ToString())
                .Equals(
                    normalizedValue,
                    StringComparison.OrdinalIgnoreCase))
            {
                result = enumValue;
                return true;
            }
        }

        return false;
    }

    private static string Normalize(string value)
    {
        return new string(
            value
                .Where(char.IsLetterOrDigit)
                .ToArray());
    }
}