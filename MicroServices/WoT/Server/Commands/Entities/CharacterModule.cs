﻿using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using WoT.Server.Services;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.Character.Ref;

namespace WoT.Server.Commands
{
    [Module(CharacterApi.MODULE)]
    public class CharacterModule :
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<Character>
    {
        public void ApplyInput(Character entity, bool assign_mandatory = true)
        {
            if(assign_mandatory)
            {
                TryFill<string>(Name, x => entity.Name = x);
            }
        }

        [Command("add")]
        [MandatoryArguments(UserId, Name)]
        public async Task AddAsync()
        {
            Character entity = new Character
            {
                UserId = GetArgUInt32(UserId),
                Name = Args[Name]
            };
            ApplyInput(entity, assign_mandatory: false);
            uint id = await AddEntityAsync(entity);
            Reply($"Character `{Args[Id]}` has been successfully added.", id);
        }

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync()
        {
            if (HasArg(Id)) //Id
            {
                ReplyEntries(SQL.Characters, ParseIdsUInt32(Args[Id]));
            }
            else
            {
                ReplyEntries(SQL.Characters);
            }
        }

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(Name)]
        public async Task ModifyAsync()
        {
            Character entity = SQL.Characters.Find(GetArgUInt32(Id));
            if (entity != null)
            {
                ApplyInput(entity);
                await UpdateEntityAsync(entity);
                Reply($"Character `{Args[Id]}` has been modified successfully.");
            }
            else
            {
                ReplyError("Character db id invalid. 404 not found.");
            }
        }

        [Command("remove")]
        [MandatoryArguments(Id)]
        public async Task RemoveAsync()
        {
            Character entity = SQL.Characters.Find(GetArgUInt32(Id));
            await RemoveEntityAsync(entity);
            Reply($"Character `{Args[Id]}` has been successfully removed.");
        }
    }
}
