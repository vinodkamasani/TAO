using TAO.Domain.Exceptions;

namespace TAO.Domain.Common;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(
        string value,
        string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{propertyName} is required.");
        }

        return value.Trim();
    }

    public static Guid AgainstEmpty(
        Guid value,
        string propertyName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{propertyName} is required.");
        }

        return value;
    }

    public static int AgainstGreaterThanZero(
    int value,
    string propertyName)
    {
        if (value <= 0)
        {
            throw new DomainException(
                $"{propertyName} must be greater than zero.");
        }

        return value;
    }

    public static long AgainstGreaterThanZero(
    long value,
    string propertyName)
    {
        if (value <= 0)
        {
            throw new DomainException($"{propertyName} must be greater than zero.");
        }

        return value;
    }
}