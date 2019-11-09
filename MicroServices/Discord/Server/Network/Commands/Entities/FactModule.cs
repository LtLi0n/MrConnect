using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Shared;
using DataServerHelpers;
using LionLibrary.Commands;
using LionLibrary.Utils;

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
                TryFill<ulong>(UserId, x => entity.UserId = x);
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
                    Content = Args[Content]
                });

        [Command("get")]
        public Task GetAsync() => WrapperGetEntitiesAsync<Fact>();

        [Command("get_random")]
        public Task GetRandomAsync()
        {
            var dbSet = SQL.Set<Fact>();
            RandomThreadSafe rts = new RandomThreadSafe();

            int count = dbSet.Count();
            int skipN = rts.Next(count);

            Reply(SQL.Set<Fact>().Skip(skipN).FirstOrDefault());

            return Task.CompletedTask;
        }

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(Content)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<Fact, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<Fact, uint>();
    }
}
