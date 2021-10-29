using System;

namespace Tide.Core
{
    public interface ISystemTime
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }

    public class SystemTime : ISystemTime
    {
        public static ISystemTime Default = new SystemTime();

        public static DateTime Now => Default.Now;
        public static DateTime UtcNow => Default.UtcNow;
        public static DateTime Today => Default.Today;

        DateTime ISystemTime.Now => DateTime.Now;
        DateTime ISystemTime.UtcNow => DateTime.UtcNow;
        DateTime ISystemTime.Today => DateTime.Today;
    }
}