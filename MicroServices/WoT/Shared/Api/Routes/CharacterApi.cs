using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.Character.Ref;

namespace WoT.Shared
{
    public class CharacterApi : ApiController, IApiControllerCRUD<Character, uint>
    {
        public const string MODULE = "characters";
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

        public UserApi UserApi { get; }
        public IApiControllerCRUD<Character, uint> CRUD => this;

        public CharacterApi(WoTConnector connector, UserApi userApi) : base(connector) 
        {
            UserApi = userApi;
        }

        public void FillPacketBucket(PacketBuilder pb, Character entity)
        {
            pb[Id] = entity.Id;
            pb[Name] = entity.Name;
            pb[Gold] = entity.Gold;
        }

        public async Task<Character> GetByDiscordIdAsync(ulong discordId)
        {
            User user = await UserApi.GetByDiscordIdAsync(discordId);
            Packet charactersPacket = await CRUD.GetAsync("x => x", $"{UserId} == {user.Id}");
            return charactersPacket.As<IEnumerable<Character>>().FirstOrDefault();
        }
    }
}
