using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public abstract class SocketUserBase : ISocketUserBase
    {
        public LionClient LionClient { get; }
        public TcpClient TcpClient => LionClient.Client;

        public SocketUserBase(LionClient client)
        {
            LionClient = client;
        }

        ///<summary>Construct a packet builder and enqueue it for sending.</summary>
        public void SendPacket(Action<PacketBuilder> action)
        {
            LionClient.SendPacket(action);
        }

        ///<summary>Enqueue for sending.</summary>
        public void SendPacket(PacketBuilder pb)
        {
            LionClient.SendPacket(pb);
        }

        public void SendMessage(string header, string message, bool success = true) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.Status = success ? StatusCode.Success : StatusCode.Failure;
                x.Content = message;
            });

        public void ReplyMessage(IPacketBase sender, string header, string message, bool success = true) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.ReplyId = sender.Id;
                x.Status = success ? StatusCode.Success : StatusCode.Failure;
                x.Content = message;
            });

        ///<summary>Sends user an object formatted as JSON.</summary>
        public void SendObjects<U>(string header, IEnumerable<U> objects) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.Status = StatusCode.Success;
                x.ContentType = PacketContentType.JSON;
                x.Content = objects != null ? JsonConvert.SerializeObject(objects) : "{}";
            });

        ///<summary>Sends user an object formatted as JSON.</summary>
        public void ReplyObjects(IPacketBase sender, string header, IEnumerable objects) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.ReplyId = sender.Id;
                x.Status = StatusCode.Success;
                x.ContentType = PacketContentType.JSON;
                x.Content = objects != null ? JsonConvert.SerializeObject(objects) : "{}";
            });

        ///<summary>Sends user an object formatted as JSON.</summary>
        public void ReplyObjects(IPacketBase sender, string header, IQueryable objects) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.ReplyId = sender.Id;
                x.Status = StatusCode.Success;
                x.ContentType = PacketContentType.JSON;
                x.Content = objects != null ? JsonConvert.SerializeObject(objects) : "{}";
            });

        ///<summary>Sends user an object formatted as JSON.</summary>
        public void SendObject(string header, object obj) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.Status = StatusCode.Success;
                x.ContentType = PacketContentType.JSON;
                x.Content = obj != null ? JsonConvert.SerializeObject(obj) : "{}";
            });

        ///<summary>Sends user an object formatted as JSON.</summary>
        public void ReplyObject(IPacketBase sender, string header, object obj) =>
            SendPacket(x =>
            {
                x.Header = header;
                x.ReplyId = sender.Id;
                x.Status = StatusCode.Success;
                x.ContentType = PacketContentType.JSON;
                x.Content = obj != null ? JsonConvert.SerializeObject(obj) : "{}";
            });
    }
}
