using System;
using System.Threading.Tasks;
using Discord.Shared;
using DataServerHelpers;
using LionLibrary.Commands;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.Guild.Ref;

namespace Discord.Server.Network.Commands.Entities
{
    [Module(GuildApi.MODULE)]
    public class GuildModule : SocketDataModuleBase<CustomCommandContext, DiscordDbContext>, IDiscordDbModule<Guild>
    {
        public void ApplyInput(Guild entity, bool assign_mandatory = true)
        {
            entity.LastUpdatedAt = DateTime.Now;

            if (assign_mandatory)
            {
                TryFill<ulong>(OwnerId, x => entity.OwnerId = x);
                TryFill<string>(Name, x => entity.Name = x);
            }

            TryFill<string>(Prefix, x => entity.Prefix = x);
        }

        [Command("add")]
        [MandatoryArguments(Id, OwnerId, Name)]
        [OptionalArguments(Prefix)]
        public Task AddAsync() =>
            WrapperAddEntityAsync<Guild, ulong>(
                () => new Guild
                {
                    Id = GetArgUInt64(Id),
                    OwnerId = GetArgUInt64(OwnerId),
                    Name = Args[Name],
                    AddedAt = DateTime.Now
                });

        [Command("get")]
        public Task GetAsync() => WrapperGetEntitiesAsync<Guild>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(OwnerId, Name, Prefix)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<Guild, ulong>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<Guild, ulong>();
    }
}
