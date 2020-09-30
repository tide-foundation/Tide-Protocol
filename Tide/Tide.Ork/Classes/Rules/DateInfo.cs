using System;

namespace Tide.Ork.Classes.Rules
{
    public static class DateInfo
    {
        public static DateTime Today => DateTime.Today;
        public static DateTime Now => DateTime.Now;
        public static long Ticks => DateTime.Now.Ticks;
        public static int Second => DateTime.Now.Second;
        public static DateTime Date => DateTime.Now.Date;
        public static int Month => DateTime.Now.Month;
        public static int Minute => DateTime.Now.Minute;
        public static int Millisecond => DateTime.Now.Millisecond;
        public static DateTimeKind Kind => DateTime.Now.Kind;
        public static int Hour => DateTime.Now.Hour;
        public static int DayOfYear => DateTime.Now.DayOfYear;
        public static string DayOfWeek => DateTime.Now.DayOfWeek.ToString().ToLower();
        public static int Day => DateTime.Now.Day;
        public static TimeSpan TimeOfDay => DateTime.Now.TimeOfDay;
        public static int Year => DateTime.Now.Year;
    }
}