using LionLibrary.Network;

namespace DataServerHelpers
{
    public class SocketUser : SocketUserBase
    {
        public SocketUser(LionClient client) : base(client) { }

        public void SendMessage(string header, string message, int id) => SendMessage<int>(header, message, id);
        public void SendMessage(string header, string message, uint id) => SendMessage<uint>(header, message, id);

        public void SendMessage(string header, string message, long id) => SendMessage<long>(header, message, id);
        public void SendMessage(string header, string message, ulong id) => SendMessage<ulong>(header, message, id);

        public void ReplyMessage(IPacketBase sender, string header, string message, int id) => ReplyMessage<int>(sender, header, message, id);
        public void ReplyMessage(IPacketBase sender, string header, string message, uint id) => ReplyMessage<uint>(sender, header, message, id);

        public void ReplyMessage(IPacketBase sender, string header, string message, long id) => ReplyMessage<long>(sender, header, message, id);
        public void ReplyMessage(IPacketBase sender, string header, string message, ulong id) => ReplyMessage<ulong>(sender, header, message, id);

        private void SendMessage<T>(string header, string message, T id) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.State = 0;
                x["Id"] = id;
                x.Content = message;
            });

        private void ReplyMessage<T>(IPacketBase sender, string header, string message, T id) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.ReplyId = sender.Id;
                x.State = 0;
                x["Id"] = id;
                x.Content = message;
            });
    }
}
