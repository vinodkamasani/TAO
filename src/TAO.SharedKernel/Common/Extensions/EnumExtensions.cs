
namespace TAO.SharedKernel.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetName<T>(this T value)
            where T : Enum
        {
            return Enum.GetName(typeof(T), value)!;
        }
    }
}
