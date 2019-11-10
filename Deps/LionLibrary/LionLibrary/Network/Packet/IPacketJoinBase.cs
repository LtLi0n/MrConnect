namespace LionLibrary.Network
{
    public interface IPacketJoinBase : IPacketBase, IReplyablePacket
    {
        long[] FragmentIds { get; }
    }
}
