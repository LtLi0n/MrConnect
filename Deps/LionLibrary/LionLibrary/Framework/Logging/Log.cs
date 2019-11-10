using System;

namespace LionLibrary.Framework
{
    public struct Log
    {
        public string Source { get; }
        public string Text { get; }
        public bool SkipLine { get; }
        public bool DisplayOrigin { get; }
        public LogSeverity Severity { get; }
        public DateTime IssuedAt { get; }

        public Log(string source, string text, bool skipLine, bool displayOrigin, LogSeverity severity, DateTime issuedAt)
        {
            Source = source;
            Text = text;
            SkipLine = skipLine;
            DisplayOrigin = displayOrigin;
            Severity = severity;
            IssuedAt = issuedAt;
        }
    }
}
