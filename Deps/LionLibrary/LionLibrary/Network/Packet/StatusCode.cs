namespace LionLibrary.Network
{
    ///<summary>Have ideas for more? Commit or fork :)</summary>
    public enum StatusCode : int
    {
        NotConnected = -2,
        Failure = -1,
        Success = 0,
        Unauthorized = 1,
        Timeout = 2,
        NotFound = 404
    }
}
