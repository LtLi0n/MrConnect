using LionLibrary.Commands;
using System.Threading.Tasks;
using SharedDiscord;
using ServerDiscord.Services;
using DataServerHelpers;

using static SharedDiscord.User.Ref;

namespace ServerDiscord.Modules
{
    [Module("user")]
    public class UserModule : SocketDataModuleBase<CustomCommandContext, DataContext>, IDataModule<User>
    {
        public void ApplyInput(User entity, bool assign_mandatory = true)
        {
            TryFill<string>(username, x => entity.Username = x);
            TryFill<string>(discriminator, x => entity.Discriminator = x);
        }

        [Command("add")]
        [MandatoryArguments(id, username, discriminator)]
        public async Task AddAsync()
        {
            User user = new User { Id = GetArgUInt64(id) };
            ApplyInput(user);

            await AddEntityAsync(user);
            LogLine($"User [Id: {user.Id}] added.", LionLibrary.Framework.LogSeverity.Verbose);
            Reply($"User '{user.Id}' has been successfully added.");
        }

        [Command("get")]
        [OptionalArguments(id)]
        public Task GetAsync()
        {
            if(HasArg(id))
            {
                ReplyEntries(SQL.Users, ParseIdsUInt64(id));
            }
            else
            {
                ReplyEntries(SQL.Users, x => x.Id);
            }

            return Task.CompletedTask;
        }

        [Command("modify")]
        [MandatoryArguments(id)]
        [OptionalArguments(username, discriminator)]
        public async Task ModifyAsync() =>
            await WrapperModifyEntityAsync<User, ulong>(
                SQL.Users,
                x => ApplyInput(x),
                $"User [{Args[id]}] modified successfully.",
                $"User [{Args[id]}] doesn't exist.");

        [Command("remove")]
        [MandatoryArguments(id)]
        public async Task RemoveAsync() =>
            await WrapperRemoveEntityAsync<User, ulong>(
                SQL.Users,
                $"User [{Args[id]}] removed successfully.",
                $"User [{Args[id]}] doesn't exist.");
    }
}
