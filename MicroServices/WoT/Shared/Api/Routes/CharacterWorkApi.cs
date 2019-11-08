using DataServerHelpers;
using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static WoT.Shared.CharacterWork.Ref;

namespace WoT.Shared
{
    public class CharacterWorkApi : EpicApiController<CharacterWork, uint>
    {
        public const string MODULE = "work";
        private static readonly string MODULE_ABSOLUTE = $"{CharacterApi.MODULE}:{MODULE}";

        public CharacterApi CharacterApi { get; }

        public CharacterWorkApi(WoTConnector connector, CharacterApi characterApi) : base(connector, MODULE_ABSOLUTE) 
        {
            CharacterApi = characterApi;
        }

        public override void FillPacketBucket(PacketBuilder pb, CharacterWork entity)
        {
            pb[CharacterId] = entity.CharacterId;
            pb[IsWorking] = entity.IsWorking;
            pb[CommittedHours] = entity.CommittedHours;
            pb[WorkFinishesAt] = entity.WorkFinishesAt;
            pb[TotalHours] = entity.TotalHours;
        }

        public async Task<CharacterWork?> GetByDiscordIdAsync(ulong discordId)
        {
            Character? character = await CharacterApi.GetByDiscordIdAsync(discordId).ConfigureAwait(false);
            if(character != null)
            {
                Packet workInfoPacket = await CRUD.GetAsync("x => x", $"{CharacterId} == {character.Id}");
                return workInfoPacket.As<IEnumerable<CharacterWork>>().FirstOrDefault();
            }
            return null;
        }
    }
}
