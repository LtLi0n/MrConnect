using LionLibrary.Commands;
using System.Threading.Tasks;
using DataServerHelpers;

namespace ServerDiscord.Modules
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
