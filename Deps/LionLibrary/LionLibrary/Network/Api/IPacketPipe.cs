using System;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public interface IPacketPipe
    {
        void SendPacket(PacketBuilder pb);
        void SendPacket(Action<PacketBuilder> pb_action);

        Task<Packet> DownloadPacketAsync(Action<PacketBuilder> pb_action);
        Task<Packet<T>> DownloadPacketAsync<T>(Action<PacketBuilder> pb_action);

        Task<Packet> DownloadPacketAsync(PacketBuilder pb);
        Task<Packet<T>> DownloadPacketAsync<T>(PacketBuilder pb);
    }
}
