using MrConnect.Boot;
using Discord;
using Discord.Rest;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrConnect.Services;
using SharedDiscord;
using DataServerHelpers;

namespace MrConnect.Discord
{
    [Group("main")]
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        public AppConfig Config { get; set; }

        public DiscordConnector Discord { get; set; }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong!");
        }

        private void ReflectServerResponse(EmbedFieldBuilder efb, int state) =>
            efb.Value = state switch
            {
                -2 => "❌ Offline",
                -3 => "⚠️ Not responding",
                _ => "🌐 Online"
            };

        [Command("status", RunMode = RunMode.Async)]
        public async Task Status()
        {
            RestUserMessage msg = null;
            EmbedBuilder eb = new EmbedBuilder();

            GuildEmoji processing_emoji = await Discord.GuildEmoji.DownloadProcessingGif();

            eb.Fields = new List<EmbedFieldBuilder>()
            {
                new EmbedFieldBuilder
                {
                    Name = $"🔷 Discord Service 🔷",
                    Value = $"{processing_emoji} Pinging...\n\u200b"
                },
                new EmbedFieldBuilder
                {
                    Name = $"⚔️ WoT Service ⚔️",
                    Value = $"{processing_emoji} Pinging..."
                }
            };

            //Discord Service ping
            {
                msg = (RestUserMessage)await ReplyAsync(embed: eb.Build());

                await PingAndModifyField(Discord, eb.Fields[0]);
                eb.Fields[0].Value += "\n\u200b";
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }

            //WoT Service ping
            /*{
                await PingAndModifyField(WoT, eb.Fields[1]);
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }*/
        }

        private async Task PingAndModifyField(ServerConnector connector, EmbedFieldBuilder field)
        {
            int state = await connector.Ping();
            ReflectServerResponse(field, state);
        }

        [Command("info")]
        public async Task Info()
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = Context.Client.CurrentUser.Username,
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Url = "https://github.com/LtLi0n/MrConnect"
                },

                Description =
                "MrConnect is currently in very early stages of development.\nn" +
                "The bot will try to achieve a multi purpose role, altough the development focus will shift towards making fun community games.\n\n" +
                "The bot itself will act like a gateway between a user and the servers, meaning that the servers should operate 24/7 independently of the bot.\n\n" +
                "The long term plan is to have many servers, each for different game, so if there are complications with one of them, rest can still operate.",

                Color = Color.Green
            };

            await ReplyAsync(embed: eb.Build());
        }
    }
}
