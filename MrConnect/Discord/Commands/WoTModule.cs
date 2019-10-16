using MrConnect.Boot;
using Discord;
using Discord.Rest;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrConnect.Services;
using DataServerHelpers;
using WoT.Shared;
using LionLibrary.Network;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using LionLibrary.Extensions;

namespace MrConnect.Discord
{
    [Group("wot"), RequireOwner]
    public class WoTModule : ModuleBase<SocketCommandContext>
    {
        public AppConfig Config { get; set; }
        public WoTConnector WoT { get; set; }

        public UserApi UserApi => WoT.GetController<UserApi>();
        public CharacterApi CharacterApi => WoT.GetController<CharacterApi>();
        public CharacterWorkApi CharacterWorkApi => WoT.GetController<CharacterWorkApi>();

        [Command("work"), Alias("w")]
        public async Task HandleWorkCommandAsync()
        {
            CharacterWork worker = await CharacterWorkApi.GetByDiscordIdAsync(Context.User.Id);
            if(worker != null)
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.Color = new Color(255, 255, 0);
                eb.AddField(x =>
                {
                    x.Name = "⛏ Mining Stats ⛏";
                    x.Value =
                    $"**Level:** {worker.Level}\n" +
                    $"**Remaining:** {worker.XpCap - worker.Xp}\n" +
                    $"**Total:** {worker.TotalHours}";
                });

                await ReplyAsync(embed: eb.Build());
            }
        }

        [Command("register")]
        public async Task RegisterAsync()
        {
            User user = await WoT.Users.GetByDiscordIdAsync(Context.User.Id);

            if(user != null)
            {
                await ReplyAsync("You are already registered.");
            }
            else
            {
                Packet response = await UserApi.CRUD.AddAsync(new User() { DiscordId = Context.User.Id });
                await ReplyAsync(response.Content);
            }
        }

        [Command("delete")]
        public async Task DeleteAsync()
        {
            Packet response = await WoT.Users.RemoveByDiscordIdAsync(Context.User.Id);
            if(response == null)
            {
                await ReplyAsync("You are not registered.");
            }
            else
            {
                await ReplyAsync(response.Content);
            }
        }
    }
}
