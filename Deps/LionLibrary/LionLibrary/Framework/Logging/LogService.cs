using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LionLibrary.Framework
{
    public class LogService : ILogService
    {
        private readonly Thread _logThread;
        private bool _isLogging;

        private readonly ConcurrentQueue<Log> _queue;

        ///<summary>A string that is always present under the last log. Useful to indicate what type of input is expected from the user.</summary>
        public string InputHinter { get; set; }
        public int InputHinterTrash { get; set; }

        ///<summary>Gets or sets a value indicating whether the cursor is visible.</summary>
        public bool CursorVisible { get => Console.CursorVisible; set => Console.CursorVisible = value; }

        ///<summary>Select <see cref="LogSeverity"/> level at which logs are displayed.</summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        public LogService(string inputHinter) : this() => InputHinter = inputHinter;

        public LogService()
        {
            _queue = new ConcurrentQueue<Log>();
            _logThread = new Thread(async () => await LogWatchDog());

            InputHinterTrash = 0;
        }

        #region Log methods, overloads

        public void Log(object instance, object text, LogSeverity severity = LogSeverity.Info) =>
            _queue.Enqueue(new Log(
                instance.GetType().Name,
                text.ToString(),
                skipLine: false,
                displayOrigin: true, 
                severity,
                DateTime.Now));

        public void Log(string source, object text, LogSeverity severity = LogSeverity.Info) =>
            _queue.Enqueue(new Log(
                source, 
                text.ToString(),
                skipLine: false,
                displayOrigin: true,
                severity,
                DateTime.Now));

        public void LogLine(object instance, object text, LogSeverity severity = LogSeverity.Info) =>
            _queue.Enqueue(new Log(
                instance.GetType().Name, 
                text.ToString(), 
                skipLine: true, 
                displayOrigin: true, 
                severity,
                DateTime.Now));

        public void LogLine(string source, object text, LogSeverity severity = LogSeverity.Info) =>
            _queue.Enqueue(new Log(
                source, 
                text.ToString(), 
                skipLine: true, 
                displayOrigin: true, 
                severity, 
                DateTime.Now));

        public void LogLine(object text, LogSeverity severity = LogSeverity.Info)
        {
            StackFrame frame = new StackFrame(1, true);
            MethodBase? mb = frame.GetMethod();
            string methodName = mb?.Name;
            string className = mb?.DeclaringType.Name;

            _queue.Enqueue(new Log(
                $"{className}->{methodName}()",
                text.ToString(),
                skipLine: true,
                displayOrigin: true,
                severity,
                DateTime.Now));
        }

        public void Write(object text, LogSeverity severity = LogSeverity.Info) =>
            _queue.Enqueue(new Log(
                null,
                text.ToString(),
                skipLine: false,
                displayOrigin: false,
                severity,
                DateTime.Now));

        public void WriteLine(object text, LogSeverity severity = LogSeverity.Info) =>
            _queue.Enqueue(new Log(
                null, 
                text.ToString(),
                skipLine: true,
                displayOrigin: false,
                severity, 
                DateTime.Now));

        #endregion

        public void Start()
        {
            _isLogging = true;
            _logThread.Start();
        }

        public void Stop()
        {
            _isLogging = false;
            _logThread.Join();
        }

        private async Task LogWatchDog()
        {
            while (_isLogging)
            {
                if (_queue.TryDequeue(out Log log))
                {
                    Log(log);
                }

                await Task.Delay(1);
            }
        }

        private void Log(Log log)
        {
            //Skip logging this entry because LogLevel is too low.
            if (log.Severity > LogLevel) return;

            lock (Console.Out)
            {
                string inputHinter = InputHinter;

                if (!string.IsNullOrEmpty(inputHinter))
                {
                    string clearCmd = string.Empty;
                    for (int i = 0; i < inputHinter.Length + InputHinterTrash; i++)
                    {
                        clearCmd += "\b \b";
                    }

                    InputHinterTrash = 0;

                    Console.Write(clearCmd);
                }
                Console.ForegroundColor = LogSeverityColors.FromLogSeverity(log.Severity);

                Console.Write(log.DisplayOrigin ?
                    $"({log.IssuedAt.TimeOfDay.ToString("hh':'mm")}) <{Enum.GetName(typeof(LogSeverity), log.Severity)}> [{log.Source}]: {log.Text}" + (log.SkipLine ? "\n" : string.Empty) :
                    log.Text + (log.SkipLine ? "\n" : string.Empty));

                if (!string.IsNullOrEmpty(inputHinter))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(inputHinter);
                }
                Console.ForegroundColor = LogSeverityColors.FromLogSeverity(LogSeverity.Info);
            }
        }
    }
}
