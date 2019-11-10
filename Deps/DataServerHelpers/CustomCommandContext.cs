using LionLibrary.Commands;
using LionLibrary.Network;

namespace DataServerHelpers
{
    public class CustomCommandContext : SocketCommandContext
    {
        public ICommandService Commands { get; }
        public IContextSocketUser User { get; }

        public CustomCommandContext(
            ICommandService commands,
            IContextSocketUser user,
            Packet packet) : base(packet)
        {
            Commands = commands;
            User = user;
        }
    }
}
