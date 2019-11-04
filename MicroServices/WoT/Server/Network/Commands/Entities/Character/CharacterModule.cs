using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.Character.Ref;
using System;
using System.Collections.Generic;

namespace WoT.Server.Network.Commands.Entities
{
    [Module(CharacterApi.MODULE)]
    public class CharacterModule :
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<Character>
    {
        public void ApplyInput(Character entity, bool assign_mandatory = true)
        {
            if (assign_mandatory)
            {
                TryFill<string>(Name, x => entity.Name = x);
            }
            
            TryFill<ulong>(Gold, x => entity.Gold = x);
        }

        [Command("add")]
        [MandatoryArguments(UserId, Name)]
        [OptionalArguments(Gold)]
        public async Task AddAsync() =>
            await WrapperAddEntityAsync<Character, uint>(
                () => new Character
                {
                    UserId = GetArgUInt32(UserId),
                    Name = Args[Name]
                });

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync() => await WrapperGetEntitiesAsync<Character>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(Name, Gold)]
        public async Task ModifyAsync() => await WrapperModifyEntityAsync<Character, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public async Task RemoveAsync() => await WrapperRemoveEntityAsync<Character, uint>();
    }
}
