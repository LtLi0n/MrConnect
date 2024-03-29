﻿using LionLibrary.Commands;
using System.Threading.Tasks;
using DataServerHelpers;

namespace WoT.Server.Network.Commands.Core
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
