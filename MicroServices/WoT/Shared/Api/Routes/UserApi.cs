using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.User.Ref;

namespace WoT.Shared
{
    public class UserApi : ApiController, IApiControllerCRUD<User, uint>
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

        public IApiControllerCRUD<User, uint> CRUD => this;

        public UserApi(WoTConnector connector) : base(connector) { }

        public void FillPacketBucket(PacketBuilder pb, User entity)
        {
            pb[Id] = entity.Id;
            pb[DiscordId] = entity.DiscordId;
            pb[IsPremium] = entity.IsPremium;
            pb[Settings] = (ulong)entity.Settings;
        }

        public async Task<User> GetByDiscordIdAsync(ulong discordId)
        {
            Packet userPacket = await CRUD.GetAsync("x => x", $"{DiscordId} == {discordId}");
            return userPacket.As<IEnumerable<User>>().FirstOrDefault();
        }

        public async Task<Packet> RemoveByDiscordIdAsync(ulong discordId)
        {
            User user = await GetByDiscordIdAsync(discordId);
            return user != null ? await CRUD.RemoveAsync(user.Id) : null;
        }
    }
}
