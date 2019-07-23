using LionLibrary.Network;
using LionLibrary.Commands;
using WarsOfTanoth.Network;

namespace WarsOfTanoth.Modules
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
