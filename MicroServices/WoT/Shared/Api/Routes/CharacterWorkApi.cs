using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.CharacterWork.Ref;

namespace WoT.Shared
{
    public class CharacterWorkApi : ApiController, IApiControllerCRUD<CharacterWork, uint>
    {
        public const string MODULE = "work";
        private static readonly string MODULE_ABSOLUTE = $"{CharacterApi.MODULE}:{MODULE}";

        private static string ADD { get; } = $"{MODULE_ABSOLUTE}.add";
        private static string GET { get; } = $"{MODULE_ABSOLUTE}.get";
        private static string MODIFY { get; } = $"{MODULE_ABSOLUTE}.modify";
        private static string REMOVE { get; } = $"{MODULE_ABSOLUTE}.remove";

        public string IdTag => Id;
        public string SelectTag => Select;
        public string WhereTag => Where;

        public string AddRoute => ADD;
        public string GetRoute => GET;
        public string ModifyRoute => MODIFY;
        public string RemoveRoute => REMOVE;

        public UserApi UserApi => Server.GetController<UserApi>();
        public IApiControllerCRUD<CharacterWork, uint> CRUD => this;

        public CharacterWorkApi(WoTConnector connector) : base(connector) { }

        public void FillPacketBucket(PacketBuilder pb, CharacterWork entity)
        {
            pb[CharacterId] = entity.CharacterId;
            pb[IsWorking] = entity.IsWorking;
            pb[StartedWorkAt] = entity.StartedWorkAt;
            pb[WorkFinishesAt] = entity.WorkFinishesAt;
            pb[TotalHours] = entity.TotalHours;
        }
    }
}
