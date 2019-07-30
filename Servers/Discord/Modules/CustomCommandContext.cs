using LionLibrary.Network;
using LionLibrary.Commands;
using ServerDiscord.Network;

namespace ServerDiscord.Modules
{
    public class CustomCommandContext : SocketCommandContext
    {
        public ICommandService Commands { get; }
        public SocketUser User { get; }

        public CustomCommandContext(
            ICommandService commands,
            SocketUser user,
            Packet packet) : base(packet)
        {
            Commands = commands;
            User = user;
        }
    }
}
