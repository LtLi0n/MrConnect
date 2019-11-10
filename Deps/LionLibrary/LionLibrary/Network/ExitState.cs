namespace LionLibrary.Network
{
    public enum ExitState : byte
    {
        ///<summary>Not specified state of the exit.</summary>
        Unknown = 0,
        ///<summary>Client unexpectedly closed the connection.</summary>
        Unexpected = 1,
        ///<summary>Server closed the connection.</summary>
        Forced = 2,
        ///<summary>Client requested to kill the connection.</summary>
        Requested = 3
    }
}
