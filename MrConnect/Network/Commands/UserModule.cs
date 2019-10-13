using DataServerHelpers;
using LionLibrary.Commands;
using MrConnect.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MrConnect.Shared;
using Discord.WebSocket;
using Discord;

using api = MrConnect.Shared.Ref.User;

namespace MrConnect.Network.Commands
{
    [Module("user")]
    public class UserModule : SocketModuleBase<CustomCommandContext>
    {
        public DiscordService Discord { get; set; }

        [Command("sendMessage")]
        [MandatoryArguments(api.UserId)]
        [OptionalArguments(api.Message, api.Embed)]
        public async Task SendMessageAsync()
        {
            ulong userId = GetArgUInt64(api.UserId);
            SocketUser user = Discord.Client.GetUser(userId);

            string content = HasArg(api.Message) ? Args[api.Message] : null;
            SharedDiscordEmbed sde = HasArg(api.Embed) ? JsonConvert.DeserializeObject<SharedDiscordEmbed>(Args[api.Embed]) : null;
            EmbedBuilder eb = sde.ToEmbedBuilder();

            IDMChannel dm = await user.GetOrCreateDMChannelAsync();

            await dm.SendMessageAsync(text: content, embed: eb?.Build());
        }
    }
}
