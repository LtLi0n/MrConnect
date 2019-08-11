using DataServerHelpers;
using LionLibrary.Commands;
using ServerDiscord.Services;
using SharedDiscord;
using System.Threading.Tasks;
using System.Linq;

using static SharedDiscord.GuildEmoji.Ref;

namespace ServerDiscord.Modules
{
    [Module("guild_emoji")]
    public class GuildEmojiModule : SocketDataModuleBase<CustomCommandContext, DataContext>, IDataModule<GuildEmoji>
    {
        public void ApplyInput(GuildEmoji entity, bool assign_mandatory = true)
        {
            TryFill<ulong>(guild_id, x => entity.GuildId = x);
            TryFill<string>(name, x => entity.Name = x);
            TryFill<bool>(animated, x => entity.IsAnimated = x);
        }

        [Command("add")]
        [MandatoryArguments(id, guild_id, name)]
        [OptionalArguments(animated)]
        public async Task AddAsync()
        {
            GuildEmoji emoji = new GuildEmoji { Id = GetArgUInt64(id) };
            ApplyInput(emoji);
            await AddEntityAsync(emoji);
            LogLine($"Added emoji {emoji}");
            Reply($"Added emoji {emoji}");
        }

        [Command("get")]
        [OptionalArguments(id, guild_id)]
        public Task GetAsync()
        {
            if(HasArg(id))
            {
                ReplyEntries(SQL.GuildEmojis, ParseIdsUInt64(id));
            }
            else if(HasArg(guild_id))
            {
                ulong _guild_id = GetArgUInt64(guild_id);
                ReplyEntries(SQL.GuildEmojis.Where(x => x.GuildId == _guild_id), x => x.Id);
            }
            else
            {
                ReplyEntries(SQL.GuildEmojis);
            }

            return Task.CompletedTask;
        }

        [Command("modify")]
        [MandatoryArguments(id)]
        [OptionalArguments(guild_id, name, animated)]
        public async Task ModifyAsync() => 
            await WrapperModifyEntityAsync<GuildEmoji, ulong>(
                SQL.GuildEmojis,
                x => ApplyInput(x),
                $"Guild emoji [{Args[id]}] modified successfully.",
                $"Guild emoji [{Args[id]}] doesn't exist.");

        [Command("remove")]
        [MandatoryArguments(id)]
        public async Task RemoveAsync() =>
            await WrapperRemoveEntityAsync<GuildEmoji, ulong>(
                SQL.GuildEmojis,
                $"Guild emoji [{Args[id]}] removed successfully.",
                $"Guild emoji [{Args[id]}] doesn't exist.");
    }
}
