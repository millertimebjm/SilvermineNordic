
namespace SilvermineNordic.Common
{
    public static class CentralTimeService
    {
        public static DateTime GetCentralTime(DateTime dateTimeUtc)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, timeZone);
        }
    }
}
