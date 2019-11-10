using DataServerHelpers;
using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.User.Ref;

namespace WoT.Shared
{
    public class UserApi : EpicApiController<User, uint>
    {
        public const string MODULE = "users";

        public UserApi(WoTConnector connector) : base(connector, MODULE) { }

        public override void FillPacket(PacketBuilder pb, User entity)
        {
            pb[Id] = entity.Id;
            pb[DiscordId] = entity.DiscordId;
            pb[IsPremium] = entity.IsPremium;
            pb[Settings] = (ulong)entity.Settings;
        }

        public async Task<User?> GetByDiscordIdAsync(ulong discordId)
        {
            var userPacket = await CRUD.GetAsync("x => x", $"{DiscordId} == {discordId}").ConfigureAwait(false);
            if(userPacket.Status == StatusCode.Success)
            {
                return userPacket.ToEntity();
            }
            return null;
        }

        public async Task<Packet?> RemoveByDiscordIdAsync(ulong discordId)
        {
            User? user = await GetByDiscordIdAsync(discordId).ConfigureAwait(false);
            if(user != null)
            {
                return await CRUD.RemoveAsync(user.Id).ConfigureAwait(false);
            }
            return null;
        }
    }
}
