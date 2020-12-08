using System;

namespace Tide.Ork.Components.AuditTrail
{
    public class LoggerConfig
    {
        public readonly string[] KeyParams;
        public readonly int AllowedLevel;
        public readonly int DeniedLevel;

        public readonly string Path;
        public readonly int MaxEntries;
        public readonly long MaxDiff;

        public LoggerConfig() {
            KeyParams = new[] { "method", "tran", "uid" };
            AllowedLevel = 2000;
            DeniedLevel = 4001;
            Path = System.IO.Path.GetTempPath();
            MaxEntries = 300;
            MaxDiff = TimeSpan.TicksPerMinute;
        }
    }
}
