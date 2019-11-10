using System.Runtime.CompilerServices;

namespace LionLibrary.Framework
{
    public interface ILogService
    {
        string InputHinter { get; set; }
        int InputHinterTrash { get; set; }
        bool CursorVisible { get; set; }
        LogSeverity LogLevel { get; set; }

        void Log(object instance, object text, LogSeverity severity = LogSeverity.Info);
        void Log(string source, object text, LogSeverity severity = LogSeverity.Info);
        void LogLine(object instance, object text, LogSeverity severity = LogSeverity.Info);
        void LogLine(string source, object text, LogSeverity severity = LogSeverity.Info);
        void LogLine(object text, LogSeverity severity = LogSeverity.Info);
        void Start();
        void Stop();
        void Write(object text, LogSeverity severity = LogSeverity.Info);
        void WriteLine(object text, LogSeverity severity = LogSeverity.Info);
    }
}