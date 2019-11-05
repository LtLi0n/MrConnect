using System;
using System.Threading.Tasks;
using Discord.Shared;
using DataServerHelpers;
using LionLibrary.Commands;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.Fact.Ref;

namespace Discord.Server.Network.Commands.Entities
{
    //This one goes to glitch, the lemon guy
    [Module(FactApi.MODULE)]
    public class FactModule : SocketDataModuleBase<CustomCommandContext, DiscordDbContext>, IDiscordDbModule<Fact>
    {
        public void ApplyInput(Fact entity, bool assign_mandatory = true)
        {
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
        public Task GetAsync() => WrapperGetEntitiesAsync<Fact>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(Content)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<Fact, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<Fact, uint>();
    }
}
