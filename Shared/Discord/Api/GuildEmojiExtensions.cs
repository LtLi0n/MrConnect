using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

using static SharedDiscord.GuildEmoji.Ref;

namespace SharedDiscord.Api
{
    public static class GuildEmojiExtensions
    {
        private static void FillPacketBucket(PacketBuilder pb, GuildEmoji entity)
        {
            pb[id] = entity.Id;
            pb[guild_id] = entity.GuildId;
            pb[name] = entity.Name;
            pb[animated] = entity.IsAnimated;
        }

        public static async Task<bool> GuildEmoji_AddAsync(this LionClient client, GuildEmoji entity) =>
            await client.AddEntityAsync("guild_emoji.add", entity, FillPacketBucket);

        public static async Task<IEnumerable<ulong>> GuildEmoji_GetAsync(this LionClient client) =>
            await client.GetEntityAsync<ulong>("guild_emoji.get");

        public static async Task<IEnumerable<GuildEmoji>> GuildEmoji_GetAsync(this LionClient client, params ulong[] ids) =>
            await client.GetEntityAsync<GuildEmoji, ulong>("guild_emoji.get", id, ids);

        public static async Task<bool> GuildEmoji_ModifyAsync(this LionClient client, GuildEmoji entity) =>
            await client.ModifyEntityAsync("guild_emoji.modify", entity, FillPacketBucket);

        public static async Task<bool> GuildEmoji_RemoveAsync(this LionClient client, ulong entity_id) =>
            await client.RemoveEntityAsync("guild_emoji.remove", entity_id, id);
    }
}
