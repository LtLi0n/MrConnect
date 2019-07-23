using MrConnect.Boot;
using Discord;
using Discord.Rest;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MrConnect.Services;

namespace MrConnect.Discord
{
    [Group("main")]
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        public IAppConfig Config { get; set; }
        public IWoTService WoT { get; set; }

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
            EmbedBuilder eb = new EmbedBuilder();

            EmbedFieldBuilder wot_field = new EmbedFieldBuilder();
            wot_field.Name = $"⚔️ {Config["servers:wot:title"]} ⚔️";
            wot_field.Value = $"{SharedEmoji.ProcessingGif} Pinging...";

            eb.Fields.Add(wot_field);
            RestUserMessage msg = (RestUserMessage)await ReplyAsync(embed: eb.Build());
            int state = await WoT.Ping();

            ReflectServerResponse(eb.Fields[0], state);

            await msg.ModifyAsync(x =>
            {
                x.Embed = eb.Build();
            });
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
                    Url = "https://github.com/LtLi0n/LionLibrary"
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
