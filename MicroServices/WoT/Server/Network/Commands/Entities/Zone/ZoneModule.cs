using LionLibrary.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using DataServerHelpers;

using static DataServerHelpers.SharedRef;
using static WoT.Shared.Zone.Ref;

namespace WoT.Server.Network.Commands.Entities
{
    [Module("zones")]
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
        public async Task AddAsync()
        {
            Zone entity = new Zone
            {
                CodeName = Args[CodeName],
                Name = Args[Name]
            };
            ApplyInput(entity, assign_mandatory: false);
            uint id = await AddEntityAsync(entity);
            Reply($"Zone `{id}` has been successfully added.", id);
        }

        [Command("get")]
        [OptionalArguments(Id)]
        public async Task GetAsync()
        {
            if (HasArg(Id)) //Id
            {
                ReplyEntries(SQL.Set<Zone>(), ParseIdsUInt32(Args[Id]));
            }
            else
            {
                ReplyEntries(SQL.Set<Zone>());
            }
        }

        [Command("modify")]
        [MandatoryArguments(Id)]
        [OptionalArguments(CodeName, Name, Description)]
        public async Task ModifyAsync()
        {
            Zone entity = SQL.Set<Zone>().Find(GetArgUInt32(Id));
            if (entity != null)
            {
                ApplyInput(entity);
                await UpdateEntityAsync(entity);
                Reply($"Zone `{Args[Id]}` has been modified successfully.");
            }
            else
            {
                ReplyError("Zone db id invalid. 404 not found.");
            }
        }

        [Command("remove")]
        [MandatoryArguments(Id)]
        public async Task RemoveAsync() => await WrapperRemoveEntityAsync<Zone, uint>();
    }
}
