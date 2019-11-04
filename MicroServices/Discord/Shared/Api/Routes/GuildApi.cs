using LionLibrary.Network;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.Guild.Ref;

namespace Discord.Shared
{
    public class GuildApi : ApiController, IApiControllerCRUD<Guild, ulong>
    {
        public const string MODULE = "guilds";
        private static string ADD { get; } = $"{MODULE}.add";
        private static string GET { get; } = $"{MODULE}.get";
        private static string MODIFY { get; } = $"{MODULE}.modify";
        private static string REMOVE { get; } = $"{MODULE}.remove";

        public string IdTag => Id;
        public string SelectTag => Select;
        public string WhereTag => Where;

        public string AddRoute => ADD;
        public string GetRoute => GET;
        public string ModifyRoute => MODIFY;
        public string RemoveRoute => REMOVE;

        public IApiControllerCRUD<Guild, ulong> CRUD => this;

        public UserApi UserApi { get; }

        public GuildApi(DiscordConnector conn, UserApi userApi) : base(conn) 
        {
            UserApi = userApi;
        }

        public void FillPacketBucket(PacketBuilder pb, Guild entity)
        {
            pb.FillDiscordEntity(entity);
            pb[OwnerId] = entity.OwnerId;
            pb[Name] = entity.Name;
            pb[Prefix] = entity.Prefix;
        }
    }
}
