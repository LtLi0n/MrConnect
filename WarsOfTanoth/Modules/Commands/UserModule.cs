using LionLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WarsOfTanoth.Modules
{
    [Module("user")]
    public class UserModule : SocketModuleBase<CustomCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            Reply("pong!");
        }
    }
}
