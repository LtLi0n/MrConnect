using System;

namespace LionLibrary.Commands
{
    ///<summary>Thrown exceptions will appear in console and will not disrupt output with the stacktrace.</summary>
    public class ManagedCommandException : Exception
    {
        public ManagedCommandException(string msg) : base(msg) { }
    }
}
