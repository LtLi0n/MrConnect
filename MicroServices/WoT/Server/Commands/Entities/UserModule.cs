using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.User.Ref;

namespace WoT.Server.Commands.Entities
{
    [Module(UserApi.MODULE)]
    public class UserModule : 
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<User>
    {
        public void ApplyInput(User entity, bool assign_mandatory = true)
        {
            TryFill<ulong>(DiscordId, x => entity.DiscordId = x);
            TryFill<bool>(IsPremium, x => entity.IsPremium = x);
            TryFill<ulong>(Settings, x => entity.Settings = (UserSettings)x);
        }

        [Command("add")]
        [MandatoryArguments(DiscordId)]
        [OptionalArguments(IsPremium, Settings)]
        public async Task AddAsync()
        {
            User entity = new User();
            ApplyInput(entity);
            uint id = await AddEntityAsync(entity);
            Reply($"User `{entity.DiscordId}` has been successfully added.", id);
        }

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync()
        {
            //ValidateCallerAsync
            //ValidateSqlAsync Read

            if (HasArg(Id)) //Id
            {
                ReplyEntries(SQL.Users, ParseIdsUInt32(Args[Id]));
            }
            else
            {
                ReplyEntries(SQL.Users);
            }
        }

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(IsPremium, Settings)]
        public async Task ModifyAsync()
        {
            User entity = SQL.Users.Find(GetArgUInt32(Id));
            if(entity != null)
            {
                ApplyInput(entity);
                await UpdateEntityAsync(entity);
                Reply($"User `{entity.DiscordId}` has been modified successfully.");
            }
            else
            {
                ReplyError("User db id invalid. 404 not found.");
            }
        }

        [Command("remove")]
        [MandatoryArguments(Id)]
        public async Task RemoveAsync()
        {
            User entity = SQL.Users.Find(GetArgUInt32(Id));
            ulong discord_id = entity.DiscordId;
            await RemoveEntityAsync(entity);
            Reply($"User `{discord_id}` has been successfully removed.");
        }
    }
}
