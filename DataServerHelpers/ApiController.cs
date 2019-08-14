using LionLibrary.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DataServerHelpers
{
    public abstract class ApiController<EntityT, PkType>
    {
        public ServerConnector Server { get; }
        public LionClient Client => Server.Client;

        public ApiController(ServerConnector server) => Server = server;

        protected abstract void FillPacketBucket(PacketBuilder pb, EntityT entity);

        public abstract Task<bool> AddAsync(EntityT entity);
        public abstract Task<IEnumerable<PkType>> GetAsync();
        public async Task<EntityT> GetAsync(PkType id) => (await GetAsync(new PkType[] { id })).FirstOrDefault();
        public abstract Task<IEnumerable<EntityT>> GetAsync(PkType[] ids);
        public abstract Task<bool> ModifyAsync(EntityT entity);
        public abstract Task<bool> RemoveAsync(PkType entity_id);
    }
}
