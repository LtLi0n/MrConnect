using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.Zone.Ref;

namespace WoT.Server.Network.Commands.Entities
{
    [Module(ZoneApi.MODULE)]
    public class ZoneModule : 
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<Zone>
    {
        public void ApplyInput(Zone entity, bool assign_mandatory = true)
        {
            if (assign_mandatory)
            {
                TryFill<string>(CodeName, x => entity.CodeName = x);
                TryFill<string>(Name, x => entity.Name = x);
            }

            TryFill<string>(Description, x => entity.Description = x);
        }

        [Command("add")]
        [MandatoryArguments(CodeName, Name)]
        [OptionalArguments(Description)]
        public Task AddAsync() => 
            WrapperAddEntityAsync<Zone, uint>(() => 
            new Zone 
            { 
                Name = Args[Name], 
                CodeName = Args[CodeName] 
            });

        [Command("get")]
        public Task GetAsync() => WrapperGetEntitiesAsync<Zone>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(CodeName, Name, Description)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<Zone, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<Zone, uint>();
    }
}
