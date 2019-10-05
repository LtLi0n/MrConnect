using LionLibrary.Network;
using System.Threading.Tasks;

using static WoT.Shared.SharedRef;
using static WoT.Shared.User.Ref;

namespace WoT.Shared
{
    public class UserApi : ApiController, IApiControllerCRUD<User, uint>
    {
        public const string MODULE = "users";
        public static string ADD { get; } = $"{MODULE}.add";
        public static string GET { get; } = $"{MODULE}.get";
        public static string MODIFY { get; } = $"{MODULE}.modify";
        public static string REMOVE { get; } = $"{MODULE}.remove";

        public UserApi(WoTConnector connector) : base(connector) { }

        public void FillPacketBucket(PacketBuilder pb, User entity)
        {
            pb[Id] = entity.Id;
            pb[DiscordId] = entity.DiscordId;
            pb[IsPremium] = entity.IsPremium;
            pb[Settings] = (ulong)entity.Settings;
        }

        public async Task<Packet> AddAsync(User entity) =>
            await Client.AddEntityAsync(ADD, entity, FillPacketBucket);

        public async Task<Packet<User>> GetAsync(uint entity_id) =>
            await Client.GetEntityAsync<User>(GET, Id, entity_id);

        public async Task<Packet> GetAsync(string select, string where = null) =>
            await Client.GetEntitiesAsync(GET, Select, select, Where, where);

        public async Task<Packet<User>> GetAsync(uint[] ids) =>
            await Client.GetEntitiesAsync<User, uint>(GET, Id, ids);

        public async Task<Packet> ModifyAsync(User entity) =>
            await Client.ModifyEntityAsync(MODIFY, entity, FillPacketBucket);

        public async Task<Packet> RemoveAsync(uint entity_id) =>
            await Client.RemoveEntityAsync(REMOVE, Id, entity_id);
    }
}
