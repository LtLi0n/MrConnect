using DataServerHelpers;
using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

using static SharedDiscord.GuildEmoji.Ref;

namespace SharedDiscord
{
    public class GuildEmojiApi : ApiController<GuildEmoji, ulong>
    {
        public GuildEmojiApi(ServerConnector server) : base(server) { }

        protected override void FillPacketBucket(PacketBuilder pb, GuildEmoji entity)
        {
            pb[id] = entity.Id;
            pb[guild_id] = entity.GuildId;
            pb[name] = entity.Name;
            pb[animated] = entity.IsAnimated;
        }

        public async Task<GuildEmoji> DownloadProcessingGif() => await GetAsync(603313837730693137);

        public override async Task<bool> AddAsync(GuildEmoji entity) =>
            await Client.AddEntityAsync("guild_emoji.add", entity, FillPacketBucket);

        public override async Task<IEnumerable<ulong>> GetAsync() =>
            await Client.GetEntityAsync<ulong>("guild_emoji.get");

        public override async Task<IEnumerable<GuildEmoji>> GetAsync(ulong[] ids) =>
            await Client.GetEntityAsync<GuildEmoji, ulong>("guild_emoji.get", id, ids);

        public override async Task<bool> ModifyAsync(GuildEmoji entity) =>
            await Client.ModifyEntityAsync("guild_emoji.modify", entity, FillPacketBucket);

        public override async Task<bool> RemoveAsync(ulong entity_id) =>
            await Client.RemoveEntityAsync("guild_emoji.remove", entity_id, id);
    }
}
