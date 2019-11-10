namespace LionLibrary.Network
{
    public interface IPacketBase
    {
        PacketType Type { get; }
        long Id { get; }
        StatusCode Status { get; }
        string Header { get; }
        byte ContentTypeValue { get; }
        string Content { get; }
    }
}
