using System;
using System.Threading.Tasks;
using DataServerHelpers;
using LionLibrary.Network;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.Fact.Ref;

namespace Discord.Shared
{
    public class FactApi : EpicApiController<Fact, uint>
    {
        public const string MODULE = "facts";

        public UserApi UserApi { get; }

        public FactApi(DiscordConnector conn, UserApi userApi) : base(conn, MODULE) 
        {
            UserApi = userApi;
        }

        public async Task<Fact> GetRandomFactAsync()
        {
            var factPacket = await CRUD.Client.DownloadPacketAsync<Fact>(x => x.Header = $"{MODULE}.get_random");
            return factPacket.ToEntity();
        }

        public override void FillPacketBucket(PacketBuilder pb, Fact entity)
        {
            pb[Id] = entity.Id;
            pb[UserId] = entity.UserId;
            pb[Content] = entity.Content;
            pb[new Arg<DateTime>(AddedAt)] = entity.AddedAt;
        }
    }
}
