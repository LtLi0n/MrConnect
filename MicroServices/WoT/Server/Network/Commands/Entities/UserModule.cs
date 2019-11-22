﻿using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.User.Ref;

namespace WoT.Server.Network.Commands.Entities
{
    [Module(UserApi.MODULE)]
    public class UserModule :
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<User>
    {
        public void ApplyInput(User entity, bool assign_mandatory = true)
        {
            if(assign_mandatory)
            {
                TryFill<ulong>(DiscordId, x => entity.DiscordId = x);
            }

            TryFill<bool>(IsPremium, x => entity.IsPremium = x);
            TryFill<ulong>(Settings, x => entity.Settings = (UserSettings)x);
        }

        [Command("add")]
        [MandatoryArguments(DiscordId)]
        [OptionalArguments(IsPremium, Settings)]
        public Task AddAsync() => 
            WrapperAddEntityAsync<User, uint>(() => 
            new User 
            { 
                DiscordId = GetArgUInt64(DiscordId) 
            });

        [Command("get")]
        [OptionalArguments(Id)]
        public Task GetAsync() => WrapperGetEntitiesAsync<User>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(IsPremium, Settings)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<User, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<User, uint>();
    }
}
