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

        public UserApi UserApi => Server.GetController<UserApi>();
        public IApiControllerCRUD<Character, uint> CRUD => this;

        public CharacterApi(WoTConnector connector) : base(connector) { }

        public void FillPacketBucket(PacketBuilder pb, Character entity)
        {
            pb[Id] = entity.Id;
            pb[Name] = entity.Name;
        }
    }
}
