using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace LionLibrary.Network
{
    public interface ISocketUserBase
    {
        LionClient LionClient { get; }
        TcpClient TcpClient { get; }

        void ReplyMessage(IPacketBase sender, string header, string message, bool success = true);
        void ReplyObject(IPacketBase sender, string header, object obj);
        void ReplyObjects(IPacketBase sender, string header, IEnumerable objects);
        void ReplyObjects(IPacketBase sender, string header, IQueryable objects);
        void SendMessage(string header, string message, bool success = true);
        void SendObject(string header, object obj);
        void SendObjects<U>(string header, IEnumerable<U> objects);
        void SendPacket(Action<PacketBuilder> action);
        void SendPacket(PacketBuilder pb);
    }
}