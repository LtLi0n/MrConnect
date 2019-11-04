using System;
using DataServerHelpers;
using LionLibrary.Network;
using Discord.Server.Network;

namespace Discord.Server
{
    public class DiscordServerService : LionServer<SocketDiscordUser, CustomCommandContext>
    {
        public DiscordServerService(IServiceProvider services) : base(services)
        {

        }
    }
}
