namespace LionLibrary.Network
{
    public interface IReplyablePacket
    {
        long ReplyId { get; }
    }
}
