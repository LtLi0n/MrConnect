using System;

namespace LionLibrary.Framework
{
    public static class LogSeverityColors
    {
        public static ConsoleColor FromLogSeverity(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical: return ConsoleColor.Red;
                case LogSeverity.Error: return ConsoleColor.Yellow;
                case LogSeverity.Warning: return ConsoleColor.DarkYellow;
                case LogSeverity.Info: return ConsoleColor.White;
                case LogSeverity.Verbose: return ConsoleColor.Gray;
                case LogSeverity.Debug: return ConsoleColor.Green;
            }

            throw new NotImplementedException();
        }
    }
}
