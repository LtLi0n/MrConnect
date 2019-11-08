using DataServerHelpers;
using LionLibrary.Network;

using static WoT.Shared.ZoneNode.Ref;

namespace WoT.Shared
{
    public class ZoneNodeApi : EpicApiController<ZoneNode, uint>
    {
        public const string MODULE = "zones";
        public static string MODULE_ABSOLUTE = $"{ZoneApi.MODULE}.{MODULE}";

        public ZoneNodeApi(WoTConnector conn) : base(conn, MODULE_ABSOLUTE) { }

        public override void FillPacketBucket(PacketBuilder pb, ZoneNode entity)
        {
            pb[ZoneId] = entity.ZoneId;

            pb[ZoneNodeNorthId] = entity.ZoneNodeNorthId;
            pb[ZoneNodeEastId] = entity.ZoneNodeEastId;
            pb[ZoneNodeSouthId] = entity.ZoneNodeSouthId;
            pb[ZoneNodeWestId] = entity.ZoneNodeWestId;

            pb[Name] = entity.Name;
            pb[Description] = entity.Description;
        }
    }
}
