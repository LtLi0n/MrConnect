using LionLibrary.Network;
using LionLibrary.Commands;
using WoT.Server.Network;
using DataServerHelpers;

namespace WoT.Server.Modules
{
    public class CustomCommandContext : SocketCommandContext
    {
        public ICommandService Commands { get; }
        public IContextSocketUser User { get; }

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
