using LionLibrary.Network;
using System.Collections.Generic;

namespace LionLibrary.Commands
{
    public class SocketCommandContext : CommandContext
    {
        public Packet Packet { get; }

        public string Content => Packet.Content;
        public IReadOnlyDictionary<string, string> Args => Packet.Args;

        public SocketCommandContext(Packet packet) => Packet = packet;
    }
}
