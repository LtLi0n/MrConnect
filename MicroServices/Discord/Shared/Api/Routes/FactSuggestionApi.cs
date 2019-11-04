using LionLibrary.Network;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.FactSuggestion.Ref;

namespace Discord.Shared
{
    public class FactSuggestionApi : ApiController, IApiControllerCRUD<FactSuggestion, uint>
    {
        public const string MODULE = "suggestions";
        public static string MODULE_ABSOLUTE = $"{FactApi.MODULE}:{MODULE}";

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

        public IApiControllerCRUD<FactSuggestion, uint> CRUD => this;

        public UserApi UserApi { get; }

        public FactSuggestionApi(DiscordConnector conn, UserApi userApi) : base(conn) 
        {
            UserApi = userApi;
        }

        public void FillPacketBucket(PacketBuilder pb, FactSuggestion entity)
        {
            pb.FillDiscordEntity(entity);

            pb[UserId] = entity.UserId;
            pb[Content] = entity.Content;    
        }
    }
}
