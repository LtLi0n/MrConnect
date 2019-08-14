using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MrConnect.Services;
using LionLibrary.Network;

using Newtonsoft.Json.Linq;
using System.Linq;
using Discord;
using MrConnect.Boot;
using System.IO;
using LionLibrary.Framework;

using SharedDiscord;

namespace MrConnect.Discord.Commands
{
    [RequireOwner]
    [Group("discord")]
    public class DiscordModule : ModuleBase<SocketCommandContext>
    {
        [Group("emoji")]
        public class MessageModule : ModuleBase<SocketCommandContext>
        {
            public DiscordConnector Discord { get; set; }
            public AppConfig Config { get; set; }
            public ILogService Logger { get; set; }

            [Command("get")]
            public async Task GetAsync(ulong? id = null)
            {
                if(id.HasValue)
                {
                    GuildEmoji result = await Discord.GuildEmoji.GetAsync(id.Value);
                    await ReplyAsync(result.ToString());
                }
                else
                {
                    var result = await Discord.GuildEmoji.GetAsync();
                    if (result.Count() == 0) await ReplyAsync("No entries.");
                    else await ReplyAsync("```json\n" + string.Join(",\n", result) + "```");
                }
            }

            [Command("sync_guild")]
            [RequireContext(ContextType.Guild)]
            public async Task AddFromGuild()
            {
                List<Task<bool>> responses = new List<Task<bool>>();

                foreach(var emote in Context.Guild.Emotes)
                {
                    Task<bool> request = Discord.GuildEmoji.AddAsync(new GuildEmoji
                    {
                        Id = emote.Id,
                        GuildId = Context.Guild.Id,
                        IsAnimated = emote.Animated,
                        Name = emote.Name
                    });

                    responses.Add(request);
                }

                int failed_count = 0;
                int total_requests = responses.Count;

                foreach(Task<bool> response in responses)
                {
                    bool state = await response;
                    if (!state) failed_count++;
                }

                await ReplyAsync($"Synced emojis from the guild. added [{total_requests - failed_count}/{total_requests}].");
            }
        }
    }
}
