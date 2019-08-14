using DataServerHelpers;
using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

using static SharedDiscord.User.Ref;

namespace SharedDiscord
{
    public class UserApi : ApiController<User, ulong>
    {
        public UserApi(ServerConnector server) : base(server) { }

        protected override void FillPacketBucket(PacketBuilder pb, User entity)
        {
            pb[id] = entity.Id;
            pb[username] = entity.Username;
            pb[discriminator] = entity.Discriminator;
        }

        public override async Task<bool> AddAsync(User entity) =>
             await Client.AddEntityAsync("user.add", entity, FillPacketBucket);

        public override async Task<IEnumerable<ulong>> GetAsync() =>
            await Client.GetEntityAsync<ulong>("user.get");

        public override async Task<IEnumerable<User>> GetAsync(ulong[] ids) =>
            await Client.GetEntityAsync<User, ulong>("user.get", id, ids);

        public override async Task<bool> ModifyAsync(User entity) =>
            await Client.ModifyEntityAsync("user.modify", entity, FillPacketBucket);

        public override async Task<bool> RemoveAsync(ulong entity_id) =>
            await Client.RemoveEntityAsync("user.remove", entity_id, id);
    }
}
