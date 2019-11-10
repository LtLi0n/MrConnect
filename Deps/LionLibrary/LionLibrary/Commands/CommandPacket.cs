using LionLibrary.Network;
using System.Threading.Tasks;

namespace LionLibrary.Commands
{
    public class CommandPacket
    {
        public Command Command { get; }
        public Packet Packet { get; }

        public CommandPacket(Command cmd, Packet packet)
        {
            Command = cmd;
            Packet = packet;
        }

        public async Task ExecuteAsync() => await Command.ExecuteAsync(new SocketCommandContext(Packet));
        public async Task ExecuteAsync(SocketCommandContext context) => await Command.ExecuteAsync(context);
    }
}
