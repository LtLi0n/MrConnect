using MrConnect.Server.Boot;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using LionLibrary.Network;
using System.Text.RegularExpressions;

using static DataServerHelpers.SharedRef;
using System.Collections.Generic;

namespace MrConnect.Server.Discord
{
    [Group("wot"), RequireOwner]
    public class WoTModule : ModuleBase<SocketCommandContext>
    {
        public AppConfig Config { get; }
        public WoTConnector WoT { get; }

        public UserApi UserApi { get; }
        public CharacterApi CharacterApi { get; }
        public CharacterWorkApi CharacterWorkApi { get; }

        public WoTModule(AppConfig config, WoTConnector wotConc)
        {
            Config = config;
            WoT = wotConc;

            UserApi = WoT.GetController<UserApi>();
            CharacterApi = WoT.GetController<CharacterApi>();
            CharacterWorkApi = WoT.GetController<CharacterWorkApi>();
        }

        [Command("work"), Alias("w")]
        public async Task HandleWorkCommandAsync()
        {
            CharacterWork worker = await CharacterWorkApi.GetByDiscordIdAsync(Context.User.Id);
            if (worker != null)
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
        public async Task RegisterAsync([Remainder] string characterName)
        {
            Character? character = await CharacterApi.GetByDiscordIdAsync(Context.User.Id);
            if(character != null)
            {
                await ReplyAsync("You are already registered.");
                return;
            }

            User? user = await UserApi.GetByDiscordIdAsync(Context.User.Id);
            uint wotUserId = user != null ? user.Id : 0;

            if (user == null)
            {
                Packet userCreateResponse = await UserApi.CRUD.AddAsync(
                    new User
                    {
                        DiscordId = Context.User.Id
                    });

                if (userCreateResponse.Status != StatusCode.Success)
                {
                    _ = ReplyAsync(userCreateResponse.Content);
                    return;
                }
                wotUserId = uint.Parse(userCreateResponse[Id]);
            }

            Packet charCreateResponse = await CharacterApi.CRUD.AddAsync(
                new Character
                {
                    UserId = wotUserId,
                    Name = characterName
                });

            await ReplyAsync(charCreateResponse.Content);
        }

        [Command("delete")]
        public async Task DeleteAsync()
        {
            Packet response = await UserApi.RemoveByDiscordIdAsync(Context.User.Id);

            if (response == null)
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
