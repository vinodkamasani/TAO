namespace TAO.SharedKernel.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string? value)
            => string.IsNullOrWhiteSpace(value);

        public static string EmptyIfNull(this string? value)
            => value ?? string.Empty;
    }
}
