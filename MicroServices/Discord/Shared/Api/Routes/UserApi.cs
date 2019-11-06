using LionLibrary.Network;
using System.Threading.Tasks;
using static DataServerHelpers.SharedRef;
using static Discord.Shared.User.Ref;

namespace Discord.Shared
{
    public class UserApi : ApiController, IApiControllerCRUD<User, ulong>
    {
        public const string MODULE = "users";
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

        public IApiControllerCRUD<User, ulong> CRUD => this;

        public UserApi(DiscordConnector conn) : base(conn) { }

        public async Task<User> GetUserAsync(ulong id)
        {
            Packet<User> userPacket = await CRUD.GetWhereAsync(id).ConfigureAwait(false);
            return userPacket.ToEntity();
        }

        public void FillPacketBucket(PacketBuilder pb, User entity)
        {
            pb.FillDiscordEntity(entity);
            pb[Username] = entity.Username;
            pb[Discriminator] = entity.Discriminator;
        }
    }
}
