using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using WoT.Server.Services;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.CharacterWork.Ref;
using System;

namespace WoT.Server.Commands.Entities
{
    [Module(CharacterWorkApi.MODULE, ParentModule = typeof(CharacterModule))]
    public class CharacterWorkModule :
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<CharacterWork>
    {
        public void ApplyInput(CharacterWork entity, bool assign_mandatory = true)
        {
            TryFill<bool>(IsWorking, x => entity.IsWorking = x);
            TryFill<DateTime>(StartedWorkAt, x => entity.StartedWorkAt = x);
            TryFill<DateTime>(WorkFinishesAt, x => entity.WorkFinishesAt = x);
            TryFill<uint>(TotalHours, x => entity.TotalHours = x);
        }

        [Command("add")]
        [MandatoryArguments(CharacterId)]
        [OptionalArguments(IsWorking, StartedWorkAt, WorkFinishesAt, TotalHours)]
        public async Task AddAsync()
        {
            CharacterWork entity = new CharacterWork { CharacterId = GetArgUInt32(CharacterId) };
            ApplyInput(entity);
            await AddEntityAsync(entity);
            Reply($"CharacterWork for character:`{Args[CharacterId]}` has been successfully added.");
        }

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync()
        {
            ReplyEntries(SQL.CharactersWork);
        }

        [Command("modify")]
        [MandatoryArguments(CharacterId)]
        [OptionalArguments(IsWorking, StartedWorkAt, WorkFinishesAt, TotalHours)]
        public async Task ModifyAsync()
        {
            CharacterWork entity = SQL.CharactersWork.Find(GetArgUInt32(CharacterId));
            if (entity != null)
            {
                ApplyInput(entity);
                await UpdateEntityAsync(entity);
                Reply($"CharacterWork `{Args[CharacterId]}` has been modified successfully.");
            }
            else
            {
                ReplyError("User db id invalid. 404 not found.");
            }
        }

        [Command("remove")]
        [MandatoryArguments(CharacterId)]
        public async Task RemoveAsync()
        {
            CharacterWork entity = SQL.CharactersWork.Find(GetArgUInt32(CharacterId));
            await RemoveEntityAsync(entity);
            Reply($"User `{Args[CharacterId]}` has been successfully removed.");
        }
    }
}
