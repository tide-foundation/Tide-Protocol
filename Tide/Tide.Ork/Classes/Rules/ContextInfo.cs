using System;

namespace Tide.Ork.Classes.Rules
{
    public static class ContextInfo
    {
        public static bool HasShutdownStarted => Environment.HasShutdownStarted;
        public static string MachineName => Environment.MachineName;
        public static string CommandLine => Environment.CommandLine;
        public static string OSVersion => Environment.OSVersion.ToString();
        public static int ProcessorCount => Environment.ProcessorCount;
        public static bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;
        public static int TickCount => Environment.TickCount;
        public static long TickCount64 => Environment.TickCount64;
        public static string UserDomainName => Environment.UserDomainName;
        public static string UserName => Environment.UserName;
        public static string Version => Environment.Version.ToString();
        public static bool Is64BitProcess => Environment.Is64BitProcess;
    }
}