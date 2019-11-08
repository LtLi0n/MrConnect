using DataServerHelpers;
using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.Character.Ref;

namespace WoT.Shared
{
    public class CharacterApi : EpicApiController<Character, uint>
    {
        public const string MODULE = "characters";

        public UserApi UserApi { get; }

        public CharacterApi(WoTConnector connector, UserApi userApi) : base(connector, MODULE) 
        {
            UserApi = userApi;
        }

        public override void FillPacketBucket(PacketBuilder pb, Character entity)
        {
            pb[Id] = entity.Id;
            pb[UserId] = entity.UserId;
            pb[ZoneNodeId] = entity.ZoneNodeId;
            pb[Name] = entity.Name;
            pb[Gold] = entity.Gold;
        }

        public async Task<Character?> GetByDiscordIdAsync(ulong discordId)
        {
            User? user = await UserApi.GetByDiscordIdAsync(discordId).ConfigureAwait(false);
            if(user != null)
            {
                Packet charactersPacket = await CRUD.GetAsync("x => x", $"{UserId} == {user.Id}");
                return charactersPacket.As<IEnumerable<Character>>().FirstOrDefault();
            }
            return null;
        }
    }
}
