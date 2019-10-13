using Discord;
using LionLibrary.Network;
using Newtonsoft.Json;
using api = MrConnect.Shared.Ref.User;

namespace MrConnect.Shared
{
    public class UserApi : ApiController
    {
        public const string MODULE = "user";

        public UserApi(MrConnectConnector connector) : base(connector) { }

        public void SendMessage(ulong userId, string message = null, SharedDiscordEmbed embed = null) =>
            Server.SendPacket(x =>
            {
                x.Header = $"{MODULE}.sendMessage";
                x[api.UserId] = userId;
                x[api.Message] = message;
                x[api.Embed] = JsonConvert.SerializeObject(embed);
            });
    }
}
