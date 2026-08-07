using System;
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

    /// <summary>
    /// Throws ArgumentOutOfRangeException if the provided integer is negative or zero.
    /// </summary>
    public static void AgainstNegativeOrZero(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");
        }
    }

    public static T AgainstNull<T>(
        T? value,
        string propertyName) where T : class
    {
        if (value is null)
        {
            throw new DomainException($"{propertyName} is required.");
        }

        return value;
    }

    public static T AgainstNull<T>(
        T? value,
        string propertyName) where T : struct
    {
        if (!value.HasValue)
        {
            throw new DomainException($"{propertyName} is required.");
        }

        return value.Value;
    }
}