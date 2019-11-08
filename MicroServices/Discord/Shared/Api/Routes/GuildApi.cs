using DataServerHelpers;
using LionLibrary.Network;

using static Discord.Shared.Guild.Ref;

namespace Discord.Shared
{
    public class GuildApi : EpicApiController<Guild, ulong>
    {
        public const string MODULE = "guilds";

        public UserApi UserApi { get; }

        public GuildApi(DiscordConnector conn, UserApi userApi) : base(conn, MODULE) 
        {
            UserApi = userApi;
        }

        public override void FillPacketBucket(PacketBuilder pb, Guild entity)
        {
            pb.FillDiscordEntity(entity);
            pb[OwnerId] = entity.OwnerId;
            pb[Name] = entity.Name;
            pb[Prefix] = entity.Prefix;
        }
    }
}
