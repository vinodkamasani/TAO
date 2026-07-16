namespace TAO.SharedKernel.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek is DayOfWeek.Saturday
                or DayOfWeek.Sunday;
        }
    }
}
