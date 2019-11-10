using LionLibrary.Framework;

namespace LionLibrary.Commands
{
    public interface ILoggableModuleBase
    {
        void LogLine(object text, LogSeverity severity = LogSeverity.Info);
        void Log(object text, LogSeverity severity = LogSeverity.Info);
    }
}
