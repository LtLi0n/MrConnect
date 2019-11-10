namespace LionLibrary.Network
{
    public enum PacketContentType : byte
    {
        ///<summary>Plain text, ideal for response headers.</summary>
        RAW,
        ///<summary>Sending a file? Let the server know!</summary>
        BINARY,
        ///<summary>Json content. Ideal for sending chunks of data.</summary>
        JSON,
        XML,
        EDI,
        EDIFACT
    }
}
