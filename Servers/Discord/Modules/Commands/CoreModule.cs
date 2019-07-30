using LionLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharedDiscord;

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
