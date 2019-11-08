using DataServerHelpers;
using LionLibrary.Network;

using static WoT.Shared.Zone.Ref;

namespace WoT.Shared
{
    public class ZoneApi : EpicApiController<Zone, uint>
    {
        public const string MODULE = "zones";

        public ZoneApi(WoTConnector conn) : base(conn, MODULE) { }

        public override void FillPacketBucket(PacketBuilder pb, Zone entity)
        {
            pb[IdTag] = entity.Id;
            pb[CodeName] = entity.CodeName;
            pb[Name] = entity.Name;
            pb[Description] = entity.Description;
        }
    }
}
