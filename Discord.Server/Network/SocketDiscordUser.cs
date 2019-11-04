using LionLibrary.Network;
using DataServerHelpers;

namespace Discord.Server.Network
{
    public class SocketDiscordUser : SocketUserBase, IContextSocketUser
    {
        public SocketDiscordUser(LionClient client) : base(client)
        {
        }
    }
}
