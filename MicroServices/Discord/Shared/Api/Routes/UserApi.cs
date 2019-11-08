using DataServerHelpers;
using LionLibrary.Network;
using System.Threading.Tasks;

using static Discord.Shared.User.Ref;

namespace Discord.Shared
{
    public class UserApi : EpicApiController<User, ulong>
    {
        public const string MODULE = "users";

        public UserApi(DiscordConnector conn) : base(conn, MODULE) {}

        public async Task<User> GetUserAsync(ulong id)
        {
            Packet<User> userPacket = await CRUD.GetWhereAsync(id).ConfigureAwait(false);
            return userPacket.ToEntity();
        }

        public override void FillPacketBucket(PacketBuilder pb, User entity)
        {
            pb.FillDiscordEntity(entity);
            pb[Username] = entity.Username;
            pb[Discriminator] = entity.Discriminator;
        }
    }
}
