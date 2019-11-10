using DataServerHelpers;
using LionLibrary.Network;

using static Discord.Shared.FactSuggestion.Ref;

namespace Discord.Shared
{
    public class FactSuggestionApi : EpicApiController<FactSuggestion, uint>
    {
        public const string MODULE = "suggestions";
        public static string MODULE_ABSOLUTE = $"{FactApi.MODULE}:{MODULE}";

        public UserApi UserApi { get; }

        public FactSuggestionApi(DiscordConnector conn, UserApi userApi) : base(conn, MODULE_ABSOLUTE) 
        {
            UserApi = userApi;
        }

        public override void FillPacket(PacketBuilder pb, FactSuggestion entity)
        {
            pb.FillDiscordEntity(entity);

            pb[UserId] = entity.UserId;
            pb[Content] = entity.Content;    
        }
    }
}
