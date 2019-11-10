using LionLibrary.Network;

namespace DataServerHelpers
{
    public interface IContextSocketUser : ISocketUserBase
    {
        void SendMessage(string header, string message, int id) => SendMessage<int>(header, message, id);
        void SendMessage(string header, string message, uint id) => SendMessage<uint>(header, message, id);

        void SendMessage(string header, string message, long id) => SendMessage<long>(header, message, id);
        void SendMessage(string header, string message, ulong id) => SendMessage<ulong>(header, message, id);

        void ReplyMessage(IPacketBase sender, string header, string message, int id) => ReplyMessage<int>(sender, header, message, id);
        void ReplyMessage(IPacketBase sender, string header, string message, uint id) => ReplyMessage<uint>(sender, header, message, id);

        void ReplyMessage(IPacketBase sender, string header, string message, long id) => ReplyMessage<long>(sender, header, message, id);
        void ReplyMessage(IPacketBase sender, string header, string message, ulong id) => ReplyMessage<ulong>(sender, header, message, id);

        private void SendMessage<T>(string header, string message, T id) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.Status = StatusCode.Success;
                x[SharedRef.Id] = id;
                x.Content = message;
            });

        internal void ReplyMessage<T>(IPacketBase sender, string header, string message, T id) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.ReplyId = sender.Id;
                x.Status = StatusCode.Success;
                x[SharedRef.Id] = id;
                x.Content = message;
            });
    }
}
