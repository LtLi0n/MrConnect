using Discord.Commands;
using System.Threading.Tasks;

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
