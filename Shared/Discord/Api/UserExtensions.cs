using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

using static SharedDiscord.User.Ref;

namespace SharedDiscord.Api
{
    public static class UserExtensions
    {
        private static void FillPacketBucket(PacketBuilder pb, User entity)
        {
            pb[id] = entity.Id;
            pb[username] = entity.Username;
            pb[discriminator] = entity.Discriminator;
        }

        public static async Task<bool> User_AddAsync(this LionClient client, User entity) =>
            await client.AddEntityAsync("user.add", entity, FillPacketBucket);

        public static async Task<IEnumerable<ulong>> User_GetAsync(this LionClient client) =>
            await client.GetEntityAsync<ulong>("user.get");

        public static async Task<IEnumerable<User>> User_GetAsync(this LionClient client, params ulong[] ids) =>
            await client.GetEntityAsync<User, ulong>("user.get", id, ids);

        public static async Task<bool> User_ModifyAsync(this LionClient client, User entity) =>
            await client.ModifyEntityAsync("user.modify", entity, FillPacketBucket);

        public static async Task<bool> User_RemoveAsync(this LionClient client, ulong entity_id) =>
            await client.RemoveEntityAsync("user.remove", entity_id, id);
    }
}
