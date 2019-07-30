using LionLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharedDiscord;
using ServerDiscord.Services;
using System.Linq;

using static SharedDiscord.User.Ref;

namespace ServerDiscord.Modules
{
    [Module("user")]
    public class UserModule : SocketModuleBase<CustomCommandContext>, IDataModule<User>
    {
        public IDataContext SQL { get; set; }

        public void ApplyInput(User entity, bool assign_mandatory = true)
        {
            TryFill<string>(Username, x => entity.Username = x);
            TryFill<string>(Discriminator, x => entity.Discriminator = x);
        }

        [Command("add")]
        [MandatoryArguments(Id, Username, Discriminator)]
        public async Task AddAsync()
        {
            User user = new User
            {
                Id = GetArgString(Id),
                Username = GetArgString(Username),
                Discriminator = GetArgString(Discriminator)
            };

            await SQL.AddEntityAsync(user);
            LogLine($"User [Id: {user.Id}] added.", LionLibrary.Framework.LogSeverity.Verbose);
            Reply($"User '{user.Id}' has been successfully added.");
        }

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync()
        {
            if(HasArg(Id))
            {
                Reply(SQL.Users.Find(Args[Id]));
            }
            else
            {
                Reply(SQL.Users.Select(x => new { x.Id, x.Username }));
            }
        }

        [Command("modify")]
        public Task ModifyAsync()
        {
            throw new NotImplementedException();
        }

        [Command("remove")]
        public Task RemoveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
