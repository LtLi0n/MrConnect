using System;
using System.Threading.Tasks;
using LionLibrary.Network;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.Fact.Ref;

namespace Discord.Shared
{
    public class FactApi : ApiController, IApiControllerCRUD<Fact, uint>
    {
        public const string MODULE = "facts";
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

        public IApiControllerCRUD<Fact, uint> CRUD => this;

        public UserApi UserApi { get; }

        public FactApi(DiscordConnector conn, UserApi userApi) : base(conn) 
        {
            UserApi = userApi;
        }

        public async Task<Fact> GetRandomFactAsync()
        {
            Packet factPacket = await CRUD.Client.DownloadPacketAsync(x => x.Header = $"{MODULE}.get_random");
            Fact fact = factPacket.As<Fact>();
            return fact;
        }

        public void FillPacketBucket(PacketBuilder pb, Fact entity)
        {
            pb[Id] = entity.Id;
            pb[UserId] = entity.UserId;
            pb[Content] = entity.Content;
            pb[new Arg<DateTime>(AddedAt)] = entity.AddedAt;
        }
    }
}
