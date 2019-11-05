using System;
using System.Threading.Tasks;
using Discord.Shared;
using DataServerHelpers;
using LionLibrary.Commands;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.FactSuggestion.Ref;

namespace Discord.Server.Network.Commands.Entities
{
    [Module(FactSuggestionApi.MODULE)]
    public class FactSuggestionModule : SocketDataModuleBase<CustomCommandContext, DiscordDbContext>, IDiscordDbModule<FactSuggestion>
    {
        public void ApplyInput(FactSuggestion entity, bool assign_mandatory = true)
        {
            entity.LastUpdatedAt = DateTime.Now;

            if (assign_mandatory)
            {
                TryFill<string>(Content, x => entity.Content = x);
            }
        }

        [Command("add")]
        [MandatoryArguments(UserId, Content)]
        public Task AddAsync() =>
            WrapperAddEntityAsync<Fact, uint>(
                () => new Fact
                {
                    UserId = GetArgUInt64(UserId),
                    Content = Args[Content],
                    AddedAt = DateTime.Now
                });

        [Command("get")]
        public Task GetAsync() => WrapperGetEntitiesAsync<FactSuggestion>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(Content)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<FactSuggestion, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<FactSuggestion, uint>();
    }
}
