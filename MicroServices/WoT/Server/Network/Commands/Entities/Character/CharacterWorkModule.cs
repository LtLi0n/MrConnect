using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.CharacterWork.Ref;
using System;

namespace WoT.Server.Network.Commands.Entities
{
    [Module(CharacterWorkApi.MODULE, ParentModule = typeof(CharacterModule))]
    public class CharacterWorkModule :
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<CharacterWork>
    {
        public void ApplyInput(CharacterWork entity, bool assign_mandatory = true)
        {
            TryFill<bool>(IsWorking, x => entity.IsWorking = x);
            TryFill<byte>(CommittedHours, x => entity.CommittedHours = x);
            TryFill<DateTime>(WorkFinishesAt, x => entity.WorkFinishesAt = x);
            TryFill<uint>(TotalHours, x => entity.TotalHours = x);
        }

        [Command("add")]
        [MandatoryArguments(CharacterId)]
        [OptionalArguments(IsWorking, CommittedHours, WorkFinishesAt, TotalHours)]
        public async Task AddAsync()
        {
            CharacterWork entity = new CharacterWork { CharacterId = GetArgUInt32(CharacterId) };
            ApplyInput(entity);
            await AddEntityAsync(entity);
            Reply($"CharacterWork for character:`{Args[CharacterId]}` has been successfully added.");
        }

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync() => await WrapperGetEntitiesAsync<CharacterWork>();

        [Command("modify")]
        [MandatoryArguments(CharacterId)]
        [OptionalArguments(IsWorking, CommittedHours, WorkFinishesAt, TotalHours)]
        public async Task ModifyAsync() => await WrapperModifyEntityAsync<CharacterWork, uint>(GetArgUInt32(CharacterId));

        [Command("remove")]
        [MandatoryArguments(CharacterId)]
        public async Task RemoveAsync() => await WrapperRemoveEntityAsync<CharacterWork, uint>();
    }
}
