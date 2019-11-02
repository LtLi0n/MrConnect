using MrConnect.Server.Boot;
using Discord;
using Discord.Rest;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrConnect.Server.Services;
using DataServerHelpers;
using WoT.Shared;
using LionLibrary.Network;
using System.Linq;
using Discord.WebSocket;
using System;
using System.Text;

namespace MrConnect.Server.Discord
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Group("test")]
        public class TestWithin : ModuleBase<SocketCommandContext>
        {
            [Group("inner")]
            public class InnerModule : ModuleBase<SocketCommandContext>
            {
                [Command]
                public async Task Test()
                {
                    await ReplyAsync("inner hello");
                }
            }
        }
    }

    [Group("outer", ParentModule = typeof(TestModule.TestWithin))]
    public class OuterModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task Test() => await ReplyAsync("outer hello");
    }
}
