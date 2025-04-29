using System;

namespace Process.Editor
{
    public static class TimeHelper
    {
        public static DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
        
        public static long GetTimestamp()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            return (long)timeSpan.TotalSeconds;
        }
    }
}