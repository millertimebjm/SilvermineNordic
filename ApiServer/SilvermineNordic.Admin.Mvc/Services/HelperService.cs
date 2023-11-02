using System;

namespace SilvermineNordic.Admin.Mvc.Services;

public static class HelperService
{
    public static decimal ConvertCelciusToFahrenheit(this decimal value)
    {
        return (value * 9) / 5 + 32;
    }

    public static decimal RoundToOneDecimal(this decimal value)
    {
        return Math.Round(value, 1);
    }

    public static DateTime? ConvertUtcToTimezone(this DateTime value, string timezoneString)
    {
        try
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneString);
            return TimeZoneInfo.ConvertTimeFromUtc(value, timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            Console.WriteLine($"The registry does not define the '{timezoneString}' Time zone.");
            return null;
        }
        catch (InvalidTimeZoneException)
        {
            Console.WriteLine($"Registry data on the '{timezoneString}' Time zone has been corrupted.");
            return null;
        }
    }
}