using System;

namespace UTNotifications
{
    /// <summary>
    /// Convenience functions for time units conversion.
    /// </summary>
    public sealed class TimeUtils
    {
        public static DateTime ToDateTime(int secondsFromNow)
        {
            return DateTime.Now.AddSeconds(secondsFromNow);
        }

        public static DateTime UnixTimestampMillisToDateTime(double unixTimestampMillis)
        {
            return unixBaseDateTime.AddMilliseconds(unixTimestampMillis).ToLocalTime();
        }

        public static int ToSecondsFromNow(DateTime dateTime)
        {
            return (int)(dateTime - DateTime.Now).TotalSeconds;
        }

        public static int MinutesToSeconds(int minutes)
        {
            return minutes * 60;
        }

        public static int HoursToSeconds(int hours)
        {
            return hours * 3600;
        }

        public static int DaysToSeconds(int days)
        {
            return days * 86400;
        }

        public static int WeeksToSeconds(int weeks)
        {
            return weeks * 604800;
        }

        private static readonly DateTime unixBaseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }
}