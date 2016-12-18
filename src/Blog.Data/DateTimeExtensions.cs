using System;

namespace Blog.Data
{
    public static class DateTimeExtensions
    {
        public static string ToHumaneString(this DateTime source, DateTime now)
        {
            var ts = new TimeSpan(now.Ticks - source.Ticks);
            var delta = Math.Abs(ts.TotalSeconds);

            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            if (delta < 0)
            {
                return "not yet";
            }

            if (delta < 24 * hour)
            {
                return "today";
            }

            if (delta < 48 * hour)
            {
                return "yesterday";
            }
            
            if (delta < 30 * day)
            {
                return ts.Days + " days ago";
            }

            if (delta < 12 * month)
            {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        public static string ToHumaneString(this DateTime source)
        {
            return ToHumaneString(source, DateTime.Now);
        }
    }
}