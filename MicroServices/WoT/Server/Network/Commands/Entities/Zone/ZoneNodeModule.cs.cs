using DataServerHelpers;
using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.ZoneNode.Ref;

namespace WoT.Server.Network.Commands.Entities
{
    [Module(ZoneNodeApi.MODULE, ParentModule = typeof(ZoneModule))]
    public class ZoneNodeModule :
        SocketDataModuleBase<CustomCommandContext, WoTDbContext>,
        IWoTDbModule<ZoneNode>
    {
        public void ApplyInput(ZoneNode entity, bool assign_mandatory = true)
        {
            if (assign_mandatory)
            {
                TryFill<uint>(ZoneId, x => entity.ZoneId = x);
                TryFill<string>(Name, x => entity.Name = x);
            }

            TryFill<string>(Description, x => entity.Description = x);

            NullTryFill<uint>(ZoneNodeNorthId, x => entity.ZoneNodeNorthId = x);
            NullTryFill<uint>(ZoneNodeEastId, x => entity.ZoneNodeEastId = x);
            NullTryFill<uint>(ZoneNodeSouthId, x => entity.ZoneNodeSouthId = x);
            NullTryFill<uint>(ZoneNodeWestId, x => entity.ZoneNodeWestId = x);
        }

        [Command("add")]
        [MandatoryArguments(ZoneId, Name)]
        public Task AddAsync() =>
            WrapperAddEntityAsync<ZoneNode, uint>(() => 
                new ZoneNode 
                { 
                    ZoneId = uint.Parse(ZoneId), 
                    Name = Args[Name] 
                });

        [Command("get")]
        public Task GetAsync() => WrapperGetEntitiesAsync<ZoneNode>();

        [Command("modify")]
        [MandatoryArguments(Id)]
        public Task ModifyAsync() => WrapperModifyEntityAsync<ZoneNode, uint>();

        [Command("remove")]
        [MandatoryArguments(Id)]
        public Task RemoveAsync() => WrapperRemoveEntityAsync<ZoneNode, uint>();
    }
}
