using LionLibrary.Commands;
using System.Threading.Tasks;
using DataServerHelpers;

namespace Discord.Server.Network.Commands.Core
{
    [Module("core")]
    public class CoreModule : SocketModuleBase<CustomCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            Reply("pong!");
        }
    }
}
