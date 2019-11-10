using LionLibrary.Framework;

namespace LionLibrary.Commands
{
    public class BetterModuleBase : ModuleBase, ILoggableModuleBase
    {        
        public void LogLine(object text, LogSeverity severity = LogSeverity.Info) => Logger.LogLine(this, text, severity);
        public void Log(object text, LogSeverity severity = LogSeverity.Info) => Logger.Log(this, text, severity);
    }
    
    public class BetterModuleBase<T> : BetterModuleBase where T : CommandContext
    {
        public new T Context => (T)base.Context;
        public BetterModuleBase() { }
    }
}
