using System;
using System.Threading.Tasks;
using Discord.Shared;
using DataServerHelpers;
using LionLibrary.Commands;

using static DataServerHelpers.SharedRef;
using static Discord.Shared.User.Ref;

namespace Discord.Server.Network.Commands.Entities
{
    [Module(UserApi.MODULE)]
    public class UserModule : SocketDataModuleBase<CustomCommandContext, DiscordDbContext>, IDiscordDbModule<User>
    {
        public void ApplyInput(User entity, bool assign_mandatory = true)
        {
            if (assign_mandatory)
            {
                entity.LastUpdatedAt = DateTime.Now;
                TryFill<string>(Username, x => entity.Username = x);
                TryFill<string>(Discriminator, x => entity.Discriminator = x);
            }
        }

        [Command("add")]
        [MandatoryArguments(Id, Username, Discriminator)]
        public Task AddAsync() =>
            WrapperAddEntityAsync<User, ulong>(
                () => new User
                {
                    Id = GetArgUInt64(Id),
                    Username = Args[Username],
                    Discriminator = Args[Discriminator]
                });

        [Command("get")]
        public Task GetAsync() => WrapperGetEntitiesAsync<User>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(Username, Discriminator)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<User, ulong>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<User, ulong>();
    }
}
